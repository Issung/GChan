# Builds GChan and zips it as GChan<version>.zip in the current directory.
# Run from the repository root. The release workflow (.github/workflows/release.yml) runs this too.
$ErrorActionPreference = "Stop"

# Extract the project's own <Version> from the .csproj file
$csproj = [xml](Get-Content -Raw "GChan\GChan.csproj")
$version = $csproj.SelectSingleNode("/Project/PropertyGroup/Version").InnerText

if (-not $version) {
    Write-Error "Version not found in the .csproj file."
    exit 1
}

$version = $version.Trim()
Write-Host "Version found: $version"

# Prepare a clean output directory
$outputDirectory = Join-Path ([System.IO.Path]::GetTempPath()) "GChanReleaseBuild\GChan$version"
if (Test-Path -Path $outputDirectory) {
    Remove-Item -Path $outputDirectory -Recurse -Force
}

# Publish the dotnet solution
Write-Host "Publishing GChan project..."
dotnet publish GChan -c Release -r win-x86 -o $outputDirectory
if ($LASTEXITCODE -ne 0) {
    Write-Error "dotnet publish failed with exit code $LASTEXITCODE."
    exit $LASTEXITCODE
}

# Pack the published output into a zip file
$zipFileName = "GChan$version.zip"
$zipFilePath = Join-Path -Path (Get-Location) -ChildPath $zipFileName

Write-Host "Creating zip file: $zipFileName"
Compress-Archive -Path "$outputDirectory" -DestinationPath $zipFilePath -Force

Write-Host "Packaging complete. Zip file saved as: $zipFileName"

# Hand the results to the GitHub Actions workflow, when running in one.
if ($env:GITHUB_OUTPUT) {
    "version=$version" >> $env:GITHUB_OUTPUT
    "zip=$zipFilePath" >> $env:GITHUB_OUTPUT
}
