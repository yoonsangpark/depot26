using System.Text.Json;

namespace SUartPGM;

static class Program
{
    [STAThread]
    static async Task Main()
    {
        ApplicationConfiguration.Initialize();

        var serverUrl = LoadUpdateServerUrl();
        if (!string.IsNullOrEmpty(serverUrl))
        {
            var checker = new UpdateChecker(serverUrl);
            if (await checker.CheckAndApplyAsync())
            {
                Application.Exit();
                return;
            }
        }

        Application.Run(new Form1());
    }

    private static string? LoadUpdateServerUrl()
    {
        try
        {
            var configPath = Path.Combine(
                Path.GetDirectoryName(Environment.ProcessPath) ?? ".",
                "update.config.json");
            if (!File.Exists(configPath))
                return null;

            var json = File.ReadAllText(configPath);
            var doc = JsonDocument.Parse(json);
            return doc.RootElement.TryGetProperty("updateServerUrl", out var prop)
                ? prop.GetString()
                : null;
        }
        catch
        {
            return null;
        }
    }
}