if (args.Length < 8)
{
    Console.WriteLine("Usage: UpdateHelper --wait-pid <pid> --source <dir> --target <dir> --restart <exe>");
    return 1;
}

int? waitPid = null;
string? source = null;
string? target = null;
string? restart = null;

for (var i = 0; i < args.Length - 1; i++)
{
    switch (args[i])
    {
        case "--wait-pid" when int.TryParse(args[i + 1], out var p):
            waitPid = p;
            break;
        case "--source":
            source = args[i + 1];
            break;
        case "--target":
            target = args[i + 1];
            break;
        case "--restart":
            restart = args[i + 1];
            break;
    }
}

if (waitPid == null || string.IsNullOrEmpty(source) || string.IsNullOrEmpty(target) || string.IsNullOrEmpty(restart))
{
    Console.WriteLine("Invalid arguments.");
    return 1;
}

try
{
    Console.WriteLine("업데이트를 적용하는 중...");
    Console.WriteLine($"  대기 PID: {waitPid}");
    Console.WriteLine($"  소스: {source}");
    Console.WriteLine($"  대상: {target}");

    // 메인 앱 종료 대기
    try
    {
        var process = System.Diagnostics.Process.GetProcessById(waitPid.Value);
        process.WaitForExit(60000);
    }
    catch
    {
        // 프로세스가 이미 종료된 경우
    }

    Thread.Sleep(500);

    // 파일 복사
    foreach (var file in Directory.GetFiles(source, "*", SearchOption.AllDirectories))
    {
        var rel = Path.GetRelativePath(source, file);
        var dest = Path.Combine(target, rel);
        var destDir = Path.GetDirectoryName(dest);
        if (!string.IsNullOrEmpty(destDir))
            Directory.CreateDirectory(destDir);
        File.Copy(file, dest, overwrite: true);
    }

    // 앱 재시작
    var psi = new System.Diagnostics.ProcessStartInfo
    {
        FileName = restart,
        UseShellExecute = true,
        WorkingDirectory = Path.GetDirectoryName(restart) ?? "."
    };
    System.Diagnostics.Process.Start(psi);

    Console.WriteLine("업데이트 완료. 앱을 재시작합니다.");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"오류: {ex.Message}");
    return 1;
}
