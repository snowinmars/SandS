param (
    [parameter(mandatory=$false)]
    [switch]$useAdobe,

    [parameter(mandatory=$false)]
    [alias('c')]
    [string]$configuration
)

if (-Not ($configuration)) {
    $configuration = 'Debug'
}

$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

### local functions

Function Get-SumMatches {
    param(
        $text,
        $regex
    )

    $groups = [regex]::Matches($text, $regex).captures.groups
    $count = 0;

    for ($i = 1; $i -lt $groups.Count; $i = $i + 2) {
        $count += $groups[$i].value
    }

    if ($count -eq 0) {
        throw 'No matches found'
    }

    Write-Output $count
}

### init

$scriptRoot = $PSScriptRoot
Write-Information "Root is '$scriptRoot'" -InformationAction Continue
Write-Information "Running as '$(whoami)'" -InformationAction Continue
Write-Information "Configuration is $configuration" -InformationAction Continue

Import-Module (Join-Path $scriptRoot "_all.psm1") -Force

$packageRoot = JoinPath $scriptRoot '..'

$totalTestsRegex = "Total tests: (?'value'\d+)"
$passedTestsRegex = "Passed: (?'value'\d+)|Skipped: (?'value'\d+)"

### flow

LogInfo 'Start tests'

$testsWatch = [System.Diagnostics.Stopwatch]::StartNew()

$output = '';
$count = 0
$testDlls = Get-ChildItem $packageRoot -Filter '*Tests*.dll'
$testDlls | % {
    $count++
    LogInfo "Testing [$count/$($testDlls.Length)] : $($_.FullName)"
    $output += & dotnet vstest $_.FullName
}

$testsWatch.Stop()

$totalTestsCount = Get-SumMatches $output $totalTestsRegex
$passedTestsCount = Get-SumMatches $output $passedTestsRegex

if ($totalTestsCount -eq $passedTestsCount) {
    LogInfo "All $totalTestsCount passes, took $($testsWatch.Elapsed.Seconds) sec"
} else {
    LogError "$totalTestsCount was discovered but only $passedTestsCount tests were passed"

    Write-Output 'vvvvvv Tests vvvvvv'
    $output
    Write-Output '^^^^^^ Tests ^^^^^^'

    throw "$totalTestsCount was discovered but only $passedTestsCount tests were passed"
}