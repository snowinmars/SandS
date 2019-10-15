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

$root = $PSScriptRoot
Write-Information "Root is '$root'" -InformationAction Continue
Write-Information "Running as '$(whoami)'" -InformationAction Continue
Write-Information "Configuration is $configuration" -InformationAction Continue

Import-Module (Join-Path $root "_all.psm1") -Force

$keys = ''

if ($useAdobe) {
    $keys += '--useAdobe=true '
}

$watchdogDllFullPath = JoinPath $root "..\UpsalesLab.Render.WatchDog.dll"

if (-Not (Test-Path $watchdogDllFullPath -Type Leaf)) {
    LogError "$watchdogDllFullPath not found"
    throw "$watchdogDllFullPath not found"
}

& 'dotnet' $watchdogDllFullPath $keys