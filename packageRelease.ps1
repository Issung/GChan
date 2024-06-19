# Define paths
$outputPath = ".\"
$tempPath = "$env:TEMP\GChanReleaseBuild"
$zipNameTemplate = "GChan{0}.zip"
$folderNameTemplate = "GChan{0}"

# Clean up previous temp directory if it exists
if (Test-Path $tempPath) {
    Remove-Item -Recurse -Force $tempPath
}

# Create new temp directory
New-Item -ItemType Directory -Force -Path $tempPath

# Get the assembly version
$assemblyInfoPath = ".\GChan\Properties\AssemblyInfo.cs"
$assemblyVersion = Select-String -Pattern 'AssemblyVersion\("([0-9\.]+)"\)' -Path $assemblyInfoPath | ForEach-Object { $_.Matches.Groups[1].Value }

if (-not $assemblyVersion) {
    Write-Error "Assembly version not found in AssemblyInfo.cs"
    exit 1
}

# Build the release configuration using msbuild
& msbuild GChan /p:Configuration=Release /p:OutputPath=$tempPath

if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed"
    exit 1
}

# Create a new directory inside tempPath with the versioned folder name
$versionedFolderName = [string]::Format($folderNameTemplate, $assemblyVersion)
$versionedFolderPath = Join-Path -Path $tempPath -ChildPath $versionedFolderName
New-Item -ItemType Directory -Force -Path $versionedFolderPath

# Move all build output to the versioned folder
Get-ChildItem -Path $tempPath -Exclude $versionedFolderName | Move-Item -Destination $versionedFolderPath

# Create the zip file
$zipFileName = [string]::Format($zipNameTemplate, $assemblyVersion)
$zipFilePath = Join-Path -Path $outputPath -ChildPath $zipFileName

Add-Type -AssemblyName System.IO.Compression.FileSystem
[System.IO.Compression.ZipFile]::CreateFromDirectory($tempPath, $zipFilePath)

Write-Host "Build and packaging complete: $zipFilePath"
