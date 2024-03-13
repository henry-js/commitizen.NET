enum LogLevel { Info; Success }
function Log {
    param (
        [LogLevel]$Level,
        [string]$Message
    )
    $Color = $Level -eq [LogLevel]::Info ? [System.ConsoleColor]::Yellow : [System.ConsoleColor]::Green
    Write-Host "LOG: " -NoNewline
    Write-Host $Message -ForegroundColor $Color
}
$Info = [LogLevel]::Info
$Success = [LogLevel]::Success

$root = Split-Path $PSScriptRoot -Parent
$zipFile = "publish.zip"
Log "Extracting $zipFile..." -Level $Info
Expand-Archive "$root\$zipFile" -DestinationPath $root -Force
Log "Zip extracted." -Level $Success
$folderName = (Get-Item $root).Name
$installPath = $env:LOCALAPPDATA + "\Programs\$folderName"
if (-not (Test-Path $installPath)) {
    New-Item -ItemType Directory -Path $installPath -Verbose
}

Log "Moving items to install directory..." -Level $Info
Copy-Item -Path "$root\publish\*" -Destination $installPath
Log "Moved." -Level $Success
Log "Cleaning install files..." -Level $Info
Remove-Item "publish" -Recurse -Force
Log "Cleaned." -Level $Success
Log "Adding path '$installPath' to %PATH% variable..." -Level $Info
$pathEnv = [System.Environment]::GetEnvironmentVariable('Path', 'User')
if ($pathEnv.Contains($installPath)) {
    Log "Path already exists." -Level $Info
    Log "Installation Done." -Level $Success
    exit
}
$pathEnv += ";$installPath"
[System.Environment]::SetEnvironmentVariable('Path', $pathEnv, 'User')
Log "Added." -Level $Success
Log "Installation Done." -Level $Success