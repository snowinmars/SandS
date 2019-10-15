$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

$root = $PSScriptRoot

Import-Module (Join-Path $root '_console.psm1') -Force

Function Get-GitInfo
{
    [CmdletBinding()]
    param (
        $settings
    )

    $commitHash = & $settings.gitExe rev-parse HEAD
    $branch = & $settings.gitExe rev-parse --abbrev-ref HEAD

    $hash = @{
        commitHash = $commitHash
        branch = $branch
    }

    $gitSettings = New-Object PSObject -Property $hash

    Write-Output $gitSettings
}

Export-ModuleMember -Function 'Get-GitInfo'