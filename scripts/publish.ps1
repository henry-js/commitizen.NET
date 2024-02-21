$root = Split-Path $PSScriptRoot -Parent
Remove-Item -Path "$root\publish*" -Force -Recurse
dotnet build "$root/src/Cli" --configuration Release --output ./publish
dotnet pack "$root/src/Cli" --output ./packages --no-build