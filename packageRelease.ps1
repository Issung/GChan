# Extract the version from the .csproj file
$csprojContent = Get-Content "GChan\GChan.csproj"
$version = $csprojContent | Select-String "<Version>(.*?)</Version>" | ForEach-Object { $_.Matches.Groups[1].Value }

if (-not $version) {
    Write-Error "Version not found in the .csproj file."
    exit 1
}

$version = $version.Trim()
Write-Host "Version found: $version"

# Prepare output directory
$outputDirectory = "$env:TEMP\GChanReleaseBuild\GChan$version"
if (-not (Test-Path -Path $outputDirectory)) {    # Create outputDirectory if it doesn't already exist.
    New-Item -Path $outputDirectory -ItemType Directory 
}
Remove-Item -Path $outputDirectory -Recurse -Force  # Clean outputDirectory.

# Publish the dotnet solution
Write-Host "Publishing GChan project..."
dotnet publish GChan -c Release -r win-x86 -o $outputDirectory

# Pack the published output into a zip file
$zipFileName = "GChan$version.zip"
$zipFilePath = Join-Path -Path (Get-Location) -ChildPath $zipFileName

Write-Host "Creating zip file: $zipFileName"
Compress-Archive -Path "$outputDirectory" -DestinationPath $zipFilePath -Force

Write-Host "Packaging complete. Zip file saved as: $zipFileName"
