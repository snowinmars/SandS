param (
    [parameter(mandatory=$false)]
    [alias('c')]
    [string]$configuration
)

$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

if (-Not ($configuration)) {
    $configuration = 'Debug'
}

$scriptRoot = $PSScriptRoot
Write-Information "Root is '$scriptRoot'" -InformationAction Continue
Write-Information "Running as '$(whoami)'" -InformationAction Continue

$initWatch = [System.Diagnostics.Stopwatch]::StartNew()

Import-Module (Join-Path $scriptRoot '_console.psm1') -Force

### local functions

Function Copy-Settings() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        $sourceFolder,
        [Parameter (Mandatory=$true)]
        $destinationFolder,
        [Parameter (Mandatory=$true)]
        $filename)
    $sourceConfig = JoinPath $solutionRoot $sourceFolder $filename
    $destinationConfig = JoinPath $destinationFolder $filename

    LogInfo "Copying config file $sourceConfig to $destinationConfig"

    Copy-Item -Path $sourceConfig -Destination $destinationConfig
}

Function CleanTemporaryFiles() {
    [CmdletBinding()]
    param()
    Get-ChildItem $solutionRoot -Recurse -Directory | `
    Where-Object { $_.FullName -like "*\obj" -or $_.FullName -like "*\bin" } | `
    Select-Object -Property 'FullName' | `
    ForEach-Object {
        Clean $_.FullName
    }
}

Function Write-LogStatistics {
    param(
        $step,
        $stepSeconds
    )
    LogInfo "   $step : $stepSeconds sec"
}

### pre build stuff

Import-Module (Join-Path $scriptRoot "_all.psm1") -Force

$solutionRoot = JoinPath $scriptRoot '..'
$configurationSettings = JoinPath $solutionRoot "_configuration\build\settings\$configuration\build.settings.psm1"

Import-Module $configurationSettings -Force

$gitInfo = Get-GitInfo $settings

$initWatch.Stop()

### clean

$output = JoinPath $scriptRoot $settings.output

$cleanWatch = [System.Diagnostics.Stopwatch]::StartNew()

LogInfo 'Pre-build cleaning'
Clean (JoinPath $output)

CleanTemporaryFiles

if (-not (Test-Path $output -Type 'Container')) {
    mkdir $output | Out-Null
}

$cleanWatch.Stop()

### build

$buildWatch = [System.Diagnostics.Stopwatch]::StartNew()

LogInfo 'Building'

$buildOutput = Build $settings $gitInfo $scriptRoot

$warningCount = $buildOutput[0]
$errorCount = $buildOutput[1]

if ($errorCount -ne 0) {
    throw 'Build was not successfull'
}

$sourceFolder = JoinPath "_migration"
$destinationFolder = JoinPath $output

Copy-Item -Path $sourceFolder -Destination $destinationFolder -Recurse

$buildWatch.Stop()

### output

if ($warningCount -eq 0) {
    LogInfo "Total build warnings: $warningCount"
} else {
    LogWarning "Total build warnings: $warningCount"
}

if ($errorCount -eq 0) {
    LogInfo "Total build errors: $errorCount"
} else {
    LogWarning "Total build errors: $errorCount"
}

if ($cleanWatch) { Write-LogStatistics 'clear' $cleanWatch.Elapsed.Seconds  }
if ($buildWatch) { Write-LogStatistics 'build' $buildWatch.Elapsed.Seconds  }
