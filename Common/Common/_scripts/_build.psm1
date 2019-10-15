$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

$root = $PSScriptRoot

Import-Module (Join-Path $root '_console.psm1') -Force
Import-Module (Join-Path $root '_localUri.psm1') -Force

Function Clean
{
    [CmdletBinding()]
    param (
        [Parameter (Mandatory=$true)]
        [string]$folder
    )

    if (-Not (Test-Path $folder)) {
        return
    }

    $path = JoinPath $folder

    try
    {
        Get-ChildItem $path -File -Recurse -InformationAction SilentlyContinue | `
            Sort-Object -Property @{Expression = {$_.FullName.Length}; Descending = $True} | `
            ForEach-Object {
                Remove-Item $_.FullName -Confirm:$false -Force
            }

        Get-ChildItem $path -Directory -Recurse -InformationAction SilentlyContinue | `
            Sort-Object -Property @{Expression = {$_.FullName.Length}; Descending = $True} | `
            ForEach-Object {
                Remove-Item $_.FullName -Confirm:$false -Force
            }
    }
    catch
    {
        LogError "Can't remove '$path'"
        throw $_
    }
}

<#
        .SYNOPSIS
            Returns two integer values:
            * warnings count
            * errors count
    #>
Function Build() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        $settings,
        [Parameter (Mandatory=$true)]
        $gitSettings,
        [Parameter (Mandatory=$true)]
        [string]$root)
    $solution = JoinPath $root "..\$($settings.solution)"

    $warningCount = 0
    $errorCount = 0

    $unused = & $settings.dotnetExe publish $solution `
        -c $settings.buildConfiguration `
        -o $settings.output `
        /p:AssemblyVersion="$($settings.version)" `
        /p:CommitHash="$($gitSettings.commitHash)" | % {

        $line = $_
        $buildInfoPrefixes = ("Configuration", "Version", ".NET", "C#", "Git hash", "Project") -join '|'
        $regex = "(.*?)($buildInfoPrefixes)(: .*)";
        $buildInfoMatch = [regex]::Match($line, $regex)

        $isLineBuildInfo = $buildInfoMatch.Success
        $isLineProjectHeader = $line -Match ".*? -> .*?UpsalesLab\.Render\..*?dll"
        $isWarning = $line -Match ': warning'
        $isError = $line -Match ': error'

        if ($isLineProjectHeader) {
            LogProjectInfo $line
        } elseif ($isLineBuildInfo) {
            $groups = $buildInfoMatch.Captures.Groups
            LogBuildInfo ($groups[1].Value) ($groups[2].Value) ($groups[3].Value)
        } elseif ($isWarning ) {
            $warningCount++
            LogWarning $line
        } elseif ($isError) {
            $errorCount++
            throw $line
        } else {
            if ($line) {
                LogInfo $line
            }
        }
    }

    Write-Output $warningCount
    Write-Output $errorCount
}

Export-ModuleMember -Function 'Build'
Export-ModuleMember -Function 'Clean'