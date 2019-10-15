param (
    [parameter(mandatory=$true)]
    [string]$newVersion
)

$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8


$scriptRoot = $PSScriptRoot
Write-Information "Root is '$scriptRoot'" -InformationAction Continue
Write-Information "Running as '$(whoami)'" -InformationAction Continue

Import-Module (Join-Path $scriptRoot "_all.psm1") -Force

$solutionRoot = JoinPath $scriptRoot '..'
$settingsRoot = JoinPath $solutionRoot "_configuration\build\settings"
$configurationSettings = JoinPath $settingsRoot 'Debug\build.settings.psm1'
$configurations = @("Debug", "Release")

Import-Module $configurationSettings -Force

$gitInfo = Get-GitInfo $settings

$currentVersion = $settings.version
if ($currentVersion -eq $newVersion) {
    LogError "Current and new versions are the same: $currentVersion == $newVersion"
    throw "Current and new versions are the same: $currentVersion == $newVersion"
}

### git

$gitNewVersion = $newVersion -Replace '\.', '-'
$releaseBranch = "release-$gitNewVersion"

cd $solutionRoot

git checkout 'release'
git pull origin 'release'
git checkout -b $releaseBranch
git merge 'master' --no-ff
git push origin $releaseBranch

cd $scriptRoot

### update version in build.settings.json

$configurations | % {
    $configuration = $_

    $configFile = JoinPath $settingsRoot $configuration 'build.settings.json'
    $regex = '"version": "\d+\.\d+\.\d+\.\d+"'
    $value = "`"version`": `"$newVersion`""

    Get-Content -path $configFile | % { $_ -Replace $regex , $value } | Out-File -Encoding utf8 "$configFile.tmp"
    Copy-Item "$configFile.tmp" -Destination $configFile
    Remove-Item "$configFile.tmp"
}

### git

cd $solutionRoot

git commit -a -m "Upgrade project version from $currentVersion to $newVersion"
git push origin $releaseBranch
git checkout 'release'
git merge $releaseBranch --no-ff
git checkout 'master'
git merge 'release' --no-ff

cd $scriptRoot

Write-Output 'Push the release and master branch manually'