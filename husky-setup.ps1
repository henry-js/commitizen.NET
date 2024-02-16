if ((Test-Path .\.config\dotnet-tools.json) -eq $false) {
    dotnet new tool-manifest
}
dotnet tool install Husky
dotnet husky install
dotnet husky add commit-msg -c @"
dotnet husky run --name commit-message-linter --args "`$1"
"@
$huskyDir = "$PSScriptRoot\.husky"
$task = [PsCustomObject]@{
    name    = "commit-message-linter"
    command = "pwsh"
    args    = [string[]]@(
        "-nop",
        ".husky/ps/commit-lint.ps1",
        "`${args}"
    )
}
$taskRunnerJson = (Get-Content "$huskyDir\task-runner.json") -join "`n"
$taskRunnerObj = ConvertFrom-Json $taskRunnerJson -Depth 8
$taskRunnerObj.tasks += $task

$taskRunnerObj | ConvertTo-Json -Depth 8 | Out-File -FilePath "$huskyDir\task-runner.json" -Force

$commitizenFile = "$huskyDir\ps\commit-lint.ps1"

New-Item $commitizenFile -Force

@'
foreach ($arg in $args) {
    $i = 1
    Write-Host "Arg {$i}: $arg"
}
$types = @{
    build    = "`u{1F3D7}"   #     build    = '🛠'
    feat     = "`u{2728}"    #     feat     = '✨'
    ci       = "`u{1F477}"   #     ci       = '👷'
    chore    = "`u{1F6A7}"   #     chore    = '🧹'
    docs     = "`u{1F4DD}"   #     docs     = '📝'
    fix      = "`u{1F41B}"   #     fix      = '🐛'
    perf     = "`u{26A1}"    #     perf     = '⚡'
    refactor = "`u{267B}"    #     refactor = '♻️'
    revert   = "`u{23EA}"    #     revert   = '⏪'
    style    = "`u{1F484}"   #     style    = '💄'
    test     = "`u{1F9EA}"   #     test     = '🧪'
}
$joinedTypes = $types.Keys -join '|'
$pattern = "^(?=.{1,90}$)(?<type>$joinedTypes)(?:\(.+\))*(?::).{4,}(?:#\d+)*(?<![\.\s])$"

if (Test-Path $args[0]) {
    $msg = Get-Content $args[0]
    Write-Host "CUR MSG: $msg"
}
if ($msg -is [array]) {
    $header = $msg[0]
    $isMultiLine = $true
}
else {
    $header = $msg
}

if ($header -match $pattern) {
    $newHeader = $types[$Matches.type] + " " + $header
    Write-Host "NEW MSG: $newHeader"
    $newHeader | Out-File $args[0]
    Exit 0
}

Write-Host "Invalid commit message" -ForegroundColor Red
Write-Host "e.g: 'feat(scope): subject' or 'fix: subject'"
Write-Host "Valid types: $($joinedTypes)"
Write-Host "more info: https://www.conventionalcommits.org/en/v1.0.0/"

Exit 1
'@ | Out-File $commitizenFile -Force

git add .
Write-Host "Example commit message:" -ForegroundColor Magenta
Write-Host "chore: set up husky.NET and commit lints" -ForegroundColor Green
Write-Host "Valid types: `n$joinedTypes" -ForegroundColor Green