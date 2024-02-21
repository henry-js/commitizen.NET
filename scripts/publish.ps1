$root = Split-Path $PSScriptRoot -Parent
Remove-Item -Path "$root\publish\*" -Force -Recurse
# Remove-Item -Path "$root\packages\*" -Force -Recurse
dotnet build "$root/src/Cli" --configuration Release --output ./publish

dotnet pack "$root/src/Cli" --output ./packages --no-build
$version = dotnet minver
$package = Get-Item "packages\*$version.nupkg"

# if ($null -eq $env:NUGET_API_KEY) {
#     throw "NUGET_API_KEY environment variable is not set.";
#     exit;
# }
# dotnet nuget push $package --api-key $env:NUGET_API_KEY --source https://api.nuget.org/v3/index.json