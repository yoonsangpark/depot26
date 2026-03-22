// SUartPGM 감시 프로그램 (Wrapper)
// SUartPGM.exe를 실행하고, 비정상/정상 종료 시 자동으로 재시작합니다.
// 부팅 시 이 Wrapper를 실행하면 SUartPGM이 항상 동작하도록 유지할 수 있습니다.

const string TargetExe = "SUartPGM.exe";
const int RestartDelayMs = 2000; // 재시작 전 대기 시간 (크래시 루프 방지)

var appDir = Path.GetDirectoryName(Environment.ProcessPath) ?? AppContext.BaseDirectory;
var targetPath = Path.Combine(appDir, TargetExe);

if (!File.Exists(targetPath))
{
    // 로그 파일에 기록 (콘솔 없이 실행되므로)
    try
    {
        var logPath = Path.Combine(appDir, "SUartPGMWrapper.log");
        File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {TargetExe} not found at {targetPath}\r\n");
    }
    catch { }
    return 1;
}

while (true)
{
    try
    {
        using var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = targetPath,
            WorkingDirectory = appDir,
            UseShellExecute = false,
            CreateNoWindow = false
        };
        process.Start();
        process.WaitForExit();
    }
    catch (Exception ex)
    {
        try
        {
            var logPath = Path.Combine(appDir, "SUartPGMWrapper.log");
            File.AppendAllText(logPath, $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] Start failed: {ex.Message}\r\n");
        }
        catch { }
    }

    Thread.Sleep(RestartDelayMs);
}
