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

Function Format-FileSize() {
    Param ([int]$size)
    If ($size -gt 1TB) {[string]::Format("{0:0.00} TB", $size / 1TB)}
    ElseIf ($size -gt 1GB) {[string]::Format("{0:0.00} GB", $size / 1GB)}
    ElseIf ($size -gt 1MB) {[string]::Format("{0:0.00} MB", $size / 1MB)}
    ElseIf ($size -gt 1KB) {[string]::Format("{0:0.00} kB", $size / 1KB)}
    ElseIf ($size -gt 0) {[string]::Format("{0:0.00} B", $size)}
    Else { throw 'Wrong input' }
}

Function Format-PowershellCommand() {
    Param ([string]$command)
    "powershell.exe -NoProfile -ExecutionPolicy ByPass -Command `"$command`""
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

$solutionRoot = JoinPath $scriptRoot '..\..'
$configurationSettings = JoinPath $solutionRoot "_configuration\build\settings\$configuration\build.settings.psm1" -shouldExists

Import-Module $configurationSettings -Force

$initWatch.Stop()

### packaging

$output = JoinPath $scriptRoot '..' -shouldExists
$packageName = '_package.tar'
$deploymentPackage = JoinPath $output $packageName

& $settings.tar -cvf $deploymentPackage --exclude $packageName -C $output "*"

$deploymentPackageSize = Format-FileSize ((Get-Item $deploymentPackage).Length)

### deploy

$remoteDeployWatch = [System.Diagnostics.Stopwatch]::StartNew()

LogInfo 'Remote deploy'

$settings.remoteMachines | % {
    $machine = $_
    $rsaPem = JoinPath $settings.sshPemRoot $machine.pemName -shouldExists

    LogInfo "Found public rsa key '$rsaPem'"

    ###

    $cmd =  " if (Test-Path $($settings.remoteDeployFolder) -Type Container) { " +
            " rm $($settings.remoteDeployFolder) -Recurse -Force ; " +
            " } " +
            " mkdir $($settings.remoteDeployFolder) > `$null ; "

    LogInfo 'ssh session #1 (clean) open'
    & $settings.ssh "$($machine.login)@$($machine.ip)" -i $rsaPem (Format-PowershellCommand $cmd)
    LogInfo 'ssh session #1 (clean) close'

    ###

    LogInfo 'ssh session #2 (copy) open'
    & $settings.scp -r -i "$rsaPem" "$deploymentPackage" "$($machine.login)@$($machine.ip):$($settings.remoteDeployFolder)"
    LogInfo 'ssh session #2 (copy) close'

    ###

    $cmd = " & $($settings.tar) -xvf $($settings.remoteDeployFolder)\$packageName -C $($settings.remoteDeployFolder) "

    LogInfo 'ssh session #3 (install) open'
    & $settings.ssh "$($machine.login)@$($machine.ip)" -i $rsaPem (Format-PowershellCommand $cmd)
    LogInfo 'ssh session #3 (install) close'

    ###

    $cmd = JoinPath $settings.remoteDeployFolder '_scripts\test.ps1'

    LogInfo 'ssh session #4 (tests) open'
    & $settings.ssh "$($machine.login)@$($machine.ip)" -i $rsaPem (Format-PowershellCommand $cmd)
    LogInfo 'ssh session #4 (tests) close'
}

$remoteDeployWatch.Stop()

if ($remoteDeployWatch) { Write-LogStatistics 'remote deploy' $remoteDeployWatch.Elapsed.Seconds }
if ($remoteDeployWatch) { LogInfo "     deployment package size: $deploymentPackageSize" }