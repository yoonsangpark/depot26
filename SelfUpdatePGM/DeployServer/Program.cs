var builder = WebApplication.CreateBuilder(args);

var app = builder.Build();

// 정적 파일 제공 (releases 폴더)
var releasesPath = Path.Combine(AppContext.BaseDirectory, "releases");
if (!Directory.Exists(releasesPath))
    Directory.CreateDirectory(releasesPath);

// releases 폴더에서 정적 파일 제공 (version.json, SUartPGM.cab 등)
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new Microsoft.Extensions.FileProviders.PhysicalFileProvider(releasesPath),
    RequestPath = ""
});

app.Run();
