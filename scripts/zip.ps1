$root = Split-Path $PSScriptRoot -Parent

Compress-Archive -Path "$root\publish" -DestinationPath "$root\publish.zip" -Force
Remove-Item "$root\publish" -Recurse -Force