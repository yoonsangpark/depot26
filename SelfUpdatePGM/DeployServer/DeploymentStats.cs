namespace DeployServer;

/// <summary>
/// 배포 통계 (인메모리)
/// </summary>
public static class DeploymentStats
{
    private static long _versionCheckCount;
    private static long _downloadCount;
    private static readonly DateTime _startTime = DateTime.UtcNow;

    public static void RecordVersionCheck() => Interlocked.Increment(ref _versionCheckCount);
    public static void RecordDownload() => Interlocked.Increment(ref _downloadCount);

    public static (long VersionChecks, long Downloads, TimeSpan Uptime) GetStats()
    {
        return (Interlocked.Read(ref _versionCheckCount), Interlocked.Read(ref _downloadCount), DateTime.UtcNow - _startTime);
    }
}
