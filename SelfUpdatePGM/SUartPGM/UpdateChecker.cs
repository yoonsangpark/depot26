using System.Diagnostics;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;

namespace SUartPGM;

/// <summary>
/// 배포 서버에서 최신 버전 확인 및 업데이트
/// </summary>
public class UpdateChecker
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    private readonly string _serverBaseUrl;
    private readonly string _appDir;
    private readonly string _currentVersion;

    public UpdateChecker(string serverBaseUrl)
    {
        _serverBaseUrl = serverBaseUrl.TrimEnd('/');
        _appDir = Path.GetDirectoryName(Environment.ProcessPath) ?? ".";
        _currentVersion = GetCurrentVersion();
    }

    public static string GetCurrentVersion()
    {
        var ver = Assembly.GetExecutingAssembly().GetName().Version;
        return ver != null ? $"{ver.Major}.{ver.Minor}.{ver.Build}" : "1.0.0";
    }

    /// <summary>
    /// 업데이트 확인 및 적용
    /// </summary>
    /// <returns>업데이트 적용 시 true (앱 종료 예정), 그 외 false</returns>
    public async Task<bool> CheckAndApplyAsync(CancellationToken ct = default)
    {
        try
        {
            var serverVersion = await FetchServerVersionAsync(ct);
            if (string.IsNullOrEmpty(serverVersion))
                return false;

            if (!IsNewerVersion(serverVersion, _currentVersion))
                return false;

            var cabUrl = $"{_serverBaseUrl}/SUartPGM.cab";
            var tempDir = Path.Combine(Path.GetTempPath(), "SUartPGM_Update_" + Guid.NewGuid().ToString("N")[..8]);
            var cabPath = Path.Combine(tempDir, "SUartPGM.cab");
            var extractDir = Path.Combine(tempDir, "extract");

            Directory.CreateDirectory(tempDir);

            if (!await DownloadFileAsync(cabUrl, cabPath, ct))
            {
                CleanupTemp(tempDir);
                return false;
            }

            if (!ExtractCab(cabPath, extractDir))
            {
                CleanupTemp(tempDir);
                return false;
            }

            var updateHelper = Path.Combine(_appDir, "UpdateHelper.exe");
            if (!File.Exists(updateHelper))
            {
                CleanupTemp(tempDir);
                return false;
            }

            var psi = new ProcessStartInfo
            {
                FileName = updateHelper,
                Arguments = $"--wait-pid {Environment.ProcessId} --source \"{extractDir}\" --target \"{_appDir}\" --restart \"{Path.Combine(_appDir, "SUartPGM.exe")}\"",
                UseShellExecute = true,
                CreateNoWindow = false
            };
            Process.Start(psi);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private async Task<string?> FetchServerVersionAsync(CancellationToken ct)
    {
        var url = $"{_serverBaseUrl}/version.json";
        var response = await HttpClient.GetAsync(url, ct);
        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync(ct);
        var doc = JsonDocument.Parse(json);
        return doc.RootElement.TryGetProperty("version", out var ver) ? ver.GetString() : null;
    }

    private static bool IsNewerVersion(string server, string current)
    {
        try
        {
            var s = ParseVersion(server);
            var c = ParseVersion(current);
            if (s.Major != c.Major) return s.Major > c.Major;
            if (s.Minor != c.Minor) return s.Minor > c.Minor;
            return s.Build > c.Build;
        }
        catch
        {
            return false;
        }
    }

    private static (int Major, int Minor, int Build) ParseVersion(string v)
    {
        var parts = v.Split('.');
        var major = parts.Length > 0 && int.TryParse(parts[0], out var m) ? m : 0;
        var minor = parts.Length > 1 && int.TryParse(parts[1], out var n) ? n : 0;
        var build = parts.Length > 2 && int.TryParse(parts[2], out var b) ? b : 0;
        return (major, minor, build);
    }

    private async Task<bool> DownloadFileAsync(string url, string destPath, CancellationToken ct)
    {
        try
        {
            var response = await HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, ct);
            response.EnsureSuccessStatusCode();
            await using var fs = File.Create(destPath);
            await response.Content.CopyToAsync(fs, ct);
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool ExtractCab(string cabPath, string destDir)
    {
        try
        {
            Directory.CreateDirectory(destDir);
            var expand = Path.Combine(Environment.SystemDirectory, "expand.exe");
            var psi = new ProcessStartInfo
            {
                FileName = expand,
                Arguments = $"\"{cabPath}\" -F:* \"{destDir}\"",
                UseShellExecute = false,
                CreateNoWindow = true
            };
            var p = Process.Start(psi);
            p?.WaitForExit(30000);
            return p?.ExitCode == 0;
        }
        catch
        {
            return false;
        }
    }

    private static void CleanupTemp(string dir)
    {
        try
        {
            if (Directory.Exists(dir))
                Directory.Delete(dir, true);
        }
        catch { }
    }
}
