# SUartPGM 빌드 및 CAB 패키징 스크립트
# 실행: .\build.ps1

$ErrorActionPreference = "Stop"
$ProjectRoot = $PSScriptRoot

# 1. publish SUartPGM
Write-Host "Publishing SUartPGM..." -ForegroundColor Cyan
Push-Location $ProjectRoot\SUartPGM
dotnet publish -c Release -r win-x64 --self-contained false -o "$ProjectRoot\publish\SUartPGM"
Pop-Location

# 2. publish UpdateHelper
Write-Host "Publishing UpdateHelper..." -ForegroundColor Cyan
Push-Location $ProjectRoot\UpdateHelper
dotnet publish -c Release -r win-x64 --self-contained false -o "$ProjectRoot\publish\UpdateHelper"
Pop-Location

# 2b. publish SUartPGMWrapper
Write-Host "Publishing SUartPGMWrapper..." -ForegroundColor Cyan
Push-Location $ProjectRoot\SUartPGMWrapper
dotnet publish -c Release -r win-x64 --self-contained false -o "$ProjectRoot\publish\SUartPGMWrapper"
Pop-Location

# 3. Copy UpdateHelper.exe and SUartPGMWrapper.exe to SUartPGM publish folder
Copy-Item "$ProjectRoot\publish\UpdateHelper\UpdateHelper.exe" "$ProjectRoot\publish\SUartPGM\" -Force
Copy-Item "$ProjectRoot\publish\UpdateHelper\UpdateHelper.dll" "$ProjectRoot\publish\SUartPGM\" -Force
Copy-Item "$ProjectRoot\publish\UpdateHelper\UpdateHelper.pdb" "$ProjectRoot\publish\SUartPGM\" -ErrorAction SilentlyContinue
Copy-Item "$ProjectRoot\publish\UpdateHelper\UpdateHelper.runtimeconfig.json" "$ProjectRoot\publish\SUartPGM\" -ErrorAction SilentlyContinue

Copy-Item "$ProjectRoot\publish\SUartPGMWrapper\SUartPGMWrapper.exe" "$ProjectRoot\publish\SUartPGM\" -Force
Copy-Item "$ProjectRoot\publish\SUartPGMWrapper\SUartPGMWrapper.dll" "$ProjectRoot\publish\SUartPGM\" -Force
Copy-Item "$ProjectRoot\publish\SUartPGMWrapper\SUartPGMWrapper.pdb" "$ProjectRoot\publish\SUartPGM\" -ErrorAction SilentlyContinue
Copy-Item "$ProjectRoot\publish\SUartPGMWrapper\SUartPGMWrapper.runtimeconfig.json" "$ProjectRoot\publish\SUartPGM\" -ErrorAction SilentlyContinue

# 4. Get version from SUartPGM
$Version = (Get-Item "$ProjectRoot\publish\SUartPGM\SUartPGM.dll").VersionInfo.FileVersion
if (-not $Version) { $Version = "1.0.0" }
$Version = ($Version -replace '\.0+$', '') -replace '\.$', ''  # 1.0.0.0 -> 1.0.0
if ([string]::IsNullOrEmpty($Version)) { $Version = "1.0.0" }

# 5. Create DDF for makecab
$PublishDir = "$ProjectRoot\publish\SUartPGM"
$DdfPath = "$ProjectRoot\publish\pack.ddf"
$CabDir = "$ProjectRoot\publish\cab"
$CabPath = "$CabDir\SUartPGM.cab"

New-Item -ItemType Directory -Path $CabDir -Force | Out-Null

# CAB는 동일 파일명을 허용하지 않음 - Windows용 파일만 포함
$fileList = Get-ChildItem -Path $PublishDir -Recurse -File | ForEach-Object {
    $rel = $_.FullName.Substring($PublishDir.Length + 1).Replace('\', '/')
    # runtimes 중 win만 포함 (linux, osx, android 등 제외)
    if ($rel.StartsWith("runtimes/") -and $rel -notmatch "^runtimes/win") { $null } else { $rel }
} | Where-Object { $_ }

$ddfLines = @(
    ".OPTION EXPLICIT",
    ".Set Cabinet=on",
    ".Set Compress=on",
    ".Set CabinetNameTemplate=SUartPGM.cab",
    ".Set DiskDirectory1=$CabDir",
    ".Set SourceDir=$PublishDir",
    ""
) + ($fileList | ForEach-Object { "`"$_`"" })

$ddfLines | Out-File -FilePath $DdfPath -Encoding ASCII

# 6. Create CAB
Write-Host "Creating CAB..." -ForegroundColor Cyan
& "$env:SystemRoot\System32\makecab.exe" /F $DdfPath


# 7. Create version.json and copy to releases
$CabFile = Get-ChildItem $CabDir -Filter "*.cab" -ErrorAction SilentlyContinue | Select-Object -First 1
if (-not $CabFile) {
    Write-Host "CAB 생성 실패" -ForegroundColor Red
    exit 1
}

$CabPath = $CabFile.FullName
if ($CabFile.Name -ne "SUartPGM.cab") {
    $CabPath = Join-Path $CabDir "SUartPGM.cab"
    Move-Item $CabFile.FullName $CabPath -Force
}

$VersionJson = @{
    version = $Version
    url     = "SUartPGM.cab"
    size    = (Get-Item $CabPath).Length
} | ConvertTo-Json

$ReleasesDir = "$ProjectRoot\DeployServer\releases"
New-Item -ItemType Directory -Path $ReleasesDir -Force | Out-Null
$VersionJson | Out-File "$ReleasesDir\version.json" -Encoding UTF8
Copy-Item $CabPath "$ReleasesDir\SUartPGM.cab" -Force

Write-Host "`nDone! Version: $Version" -ForegroundColor Green
Write-Host "  CAB: $ReleasesDir\SUartPGM.cab"
Write-Host "  version.json: $ReleasesDir\version.json"
Write-Host "`n배포 서버 실행: cd DeployServer; dotnet run" -ForegroundColor Yellow
