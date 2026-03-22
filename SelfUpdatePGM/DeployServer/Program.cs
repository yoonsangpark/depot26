using DeployServer;

var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

var releasesPath = Path.Combine(AppContext.BaseDirectory, "releases");
if (!Directory.Exists(releasesPath))
    Directory.CreateDirectory(releasesPath);

// 배포 통계 수집 (version.json, CAB 다운로드)
app.Use(async (ctx, next) =>
{
    await next();
    var path = ctx.Request.Path.Value?.TrimStart('/') ?? "";
    if (path.Equals("version.json", StringComparison.OrdinalIgnoreCase))
        DeploymentStats.RecordVersionCheck();
    else if (path.Equals("SUartPGM.cab", StringComparison.OrdinalIgnoreCase))
        DeploymentStats.RecordDownload();
});

// Dashboard API
app.MapGet("/api/dashboard", () =>
{
    var (versionChecks, downloads, uptime) = DeploymentStats.GetStats();
    var versionPath = Path.Combine(releasesPath, "version.json");
    var cabPath = Path.Combine(releasesPath, "SUartPGM.cab");

    var versionInfo = new { version = "—", size = 0L, lastModified = (DateTime?)null };
    if (File.Exists(versionPath))
    {
        var verJson = System.Text.Json.JsonDocument.Parse(File.ReadAllText(versionPath));
        var root = verJson.RootElement;
        versionInfo = new
        {
            version = root.TryGetProperty("version", out var v) ? v.GetString() ?? "—" : "—",
            size = root.TryGetProperty("size", out var s) ? s.GetInt64() : 0L,
            lastModified = File.Exists(cabPath) ? (DateTime?)File.GetLastWriteTimeUtc(cabPath) : null
        };
    }

    return Results.Ok(new
    {
        version = versionInfo.version,
        cabSize = versionInfo.size,
        cabLastModified = versionInfo.lastModified,
        versionChecks,
        downloads,
        uptimeSeconds = (long)uptime.TotalSeconds,
        serverTime = DateTime.UtcNow
    });
});

// Dashboard HTML
var dashboardHtml = """
<!DOCTYPE html>
<html lang="ko">
<head>
  <meta charset="UTF-8">
  <meta name="viewport" content="width=device-width, initial-scale=1">
  <title>SUartPGM 배포 대시보드</title>
  <link rel="preconnect" href="https://fonts.googleapis.com">
  <link href="https://fonts.googleapis.com/css2?family=JetBrains+Mono:wght@400;600&family=Noto+Sans+KR:wght@400;500;700&display=swap" rel="stylesheet">
  <style>
    :root {
      --bg: #0d1117;
      --surface: #161b22;
      --border: #30363d;
      --text: #e6edf3;
      --muted: #8b949e;
      --accent: #58a6ff;
      --success: #3fb950;
      --warn: #d29922;
    }
    * { box-sizing: border-box; margin: 0; padding: 0; }
    body {
      font-family: 'Noto Sans KR', system-ui, sans-serif;
      background: var(--bg);
      color: var(--text);
      min-height: 100vh;
      padding: 1.5rem;
    }
    .container { max-width: 960px; margin: 0 auto; }
    h1 {
      font-size: 1.5rem;
      font-weight: 700;
      margin-bottom: 1.5rem;
      display: flex;
      align-items: center;
      gap: 0.5rem;
    }
    h1::before {
      content: '';
      width: 4px;
      height: 1.2em;
      background: var(--accent);
      border-radius: 2px;
    }
    .cards {
      display: grid;
      grid-template-columns: repeat(auto-fit, minmax(200px, 1fr));
      gap: 1rem;
      margin-bottom: 1.5rem;
    }
    .card {
      background: var(--surface);
      border: 1px solid var(--border);
      border-radius: 8px;
      padding: 1.25rem;
    }
    .card-label {
      font-size: 0.75rem;
      color: var(--muted);
      text-transform: uppercase;
      letter-spacing: 0.05em;
      margin-bottom: 0.25rem;
    }
    .card-value {
      font-family: 'JetBrains Mono', monospace;
      font-size: 1.5rem;
      font-weight: 600;
    }
    .card-value.accent { color: var(--accent); }
    .card-value.success { color: var(--success); }
    .section {
      background: var(--surface);
      border: 1px solid var(--border);
      border-radius: 8px;
      padding: 1.25rem;
      margin-bottom: 1rem;
    }
    .section h2 {
      font-size: 0.9rem;
      font-weight: 500;
      color: var(--muted);
      margin-bottom: 1rem;
    }
    table {
      width: 100%;
      border-collapse: collapse;
      font-size: 0.9rem;
    }
    th, td { text-align: left; padding: 0.5rem 0; border-bottom: 1px solid var(--border); }
    th { color: var(--muted); font-weight: 500; }
    .mono { font-family: 'JetBrains Mono', monospace; }
    .updated { font-size: 0.8rem; color: var(--muted); margin-top: 1rem; }
  </style>
</head>
<body>
  <div class="container">
    <h1>SUartPGM 배포 대시보드</h1>
    <div class="cards">
      <div class="card">
        <div class="card-label">현재 버전</div>
        <div class="card-value accent" id="version">—</div>
      </div>
      <div class="card">
        <div class="card-label">CAB 크기</div>
        <div class="card-value" id="cabSize">—</div>
      </div>
      <div class="card">
        <div class="card-label">버전 체크</div>
        <div class="card-value success" id="versionChecks">0</div>
      </div>
      <div class="card">
        <div class="card-label">다운로드</div>
        <div class="card-value" id="downloads">0</div>
      </div>
      <div class="card">
        <div class="card-label">서버 가동시간</div>
        <div class="card-value" id="uptime">0초</div>
      </div>
    </div>
    <div class="section">
      <h2>배포 정보</h2>
      <table>
        <tr><th>항목</th><th>값</th></tr>
        <tr><td>CAB 파일</td><td class="mono">SUartPGM.cab</td></tr>
        <tr><td>마지막 수정</td><td class="mono" id="lastModified">—</td></tr>
      </table>
    </div>
    <div class="updated" id="updated">데이터 로딩 중...</div>
  </div>
  <script>
    function formatBytes(n) {
      if (n < 1024) return n + ' B';
      if (n < 1024*1024) return (n/1024).toFixed(1) + ' KB';
      return (n/(1024*1024)).toFixed(2) + ' MB';
    }
    function formatUptime(sec) {
      if (sec < 60) return sec + '초';
      if (sec < 3600) return Math.floor(sec/60) + '분';
      const h = Math.floor(sec/3600), m = Math.floor((sec%3600)/60);
      return h + '시간 ' + m + '분';
    }
    async function fetchDashboard() {
      const r = await fetch('/api/dashboard');
      const d = await r.json();
      document.getElementById('version').textContent = d.version;
      document.getElementById('cabSize').textContent = formatBytes(d.cabSize);
      document.getElementById('versionChecks').textContent = d.versionChecks.toLocaleString();
      document.getElementById('downloads').textContent = d.downloads.toLocaleString();
      document.getElementById('uptime').textContent = formatUptime(d.uptimeSeconds);
      document.getElementById('lastModified').textContent = d.cabLastModified
        ? new Date(d.cabLastModified).toLocaleString('ko-KR') : '—';
      document.getElementById('updated').textContent = '마지막 업데이트: ' + new Date().toLocaleTimeString('ko-KR');
    }
    fetchDashboard();
    setInterval(fetchDashboard, 5000);
  </script>
</body>
</html>
""";

// Dashboard UI
app.MapGet("/", () => Results.Content(dashboardHtml, "text/html; charset=utf-8"));
app.MapGet("/dashboard", () => Results.Content(dashboardHtml, "text/html; charset=utf-8"));

// 정적 파일 (releases)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(releasesPath),
    RequestPath = ""
});

app.Run();
