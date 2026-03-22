namespace SUartPGM;

/// <summary>
/// 로그 레벨
/// </summary>
public enum LogLevel
{
    INFO,
    WARN,
    ERR
}

/// <summary>
/// RichTextBox와 파일에 로그를 저장하는 클래스
/// </summary>
public class SLog
{
    private static string LogFilePath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "SUartPGM",
        $"log_{DateTime.Now:yyyyMMdd}.txt");

    /// <summary>
    /// RichTextBox에 로그 출력
    /// </summary>
    public static void WriteToControl(RichTextBox richTextBox, LogLevel level, string message)
    {
        if (richTextBox.InvokeRequired)
        {
            richTextBox.Invoke(new Action(() => WriteToControl(richTextBox, level, message)));
            return;
        }

        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        var levelStr = level.ToString().PadRight(5);
        var line = $"[{timestamp}] [{levelStr}] {message}{Environment.NewLine}";

        richTextBox.SelectionStart = richTextBox.TextLength;
        richTextBox.SelectionLength = 0;

        switch (level)
        {
            case LogLevel.INFO:
                richTextBox.SelectionColor = Color.Black;
                break;
            case LogLevel.WARN:
                richTextBox.SelectionColor = Color.Orange;
                break;
            case LogLevel.ERR:
                richTextBox.SelectionColor = Color.Red;
                break;
        }

        richTextBox.AppendText(line);
        richTextBox.SelectionColor = richTextBox.ForeColor;
        richTextBox.ScrollToCaret();
    }

    /// <summary>
    /// 파일에 로그 저장
    /// </summary>
    public static void WriteToFile(LogLevel level, string message)
    {
        try
        {
            var dir = Path.GetDirectoryName(LogFilePath);
            if (!string.IsNullOrEmpty(dir))
                Directory.CreateDirectory(dir);

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff");
            var line = $"[{timestamp}] [{level}] {message}{Environment.NewLine}";
            File.AppendAllText(LogFilePath, line);
        }
        catch
        {
            // 파일 쓰기 실패 시 무시
        }
    }

    /// <summary>
    /// 로그 출력 (RichTextBox + 파일)
    /// </summary>
    public static void Write(RichTextBox richTextBox, LogLevel level, string message)
    {
        WriteToControl(richTextBox, level, message);
        WriteToFile(level, message);
    }
}
