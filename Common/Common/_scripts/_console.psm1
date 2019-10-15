$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

$root = $PSScriptRoot

Import-Module (Join-Path $root '_print.psm1') -Force

Function LogBuildInfo() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$prefix,
        [Parameter (Mandatory=$true)]
        [string]$title,
        [Parameter (Mandatory=$true)]
        [string]$postfix
    )
    $str = $prefix + (Printx $title -u) + $postfix

    Write-Information $str -InformationAction Continue
}

Function LogProjectInfo() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$line
    )
    $str = Printx $line -fc 'blue'
    Write-Information $str -InformationAction Continue
}

Function LogWarning {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$str1,
        [string]$str2,
        [string]$str3)
    $input = $str1

    if (-Not (IsEmptyString($str2))) {
        $input += " $str2"
    }

    if (-Not (IsEmptyString($str3))) {
        $input += " $str3"
    }

    $str = Printx $input -fc 'navy'
    Write-Warning $str -InformationAction Continue
}

Function LogError() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$str1,
        [string]$str2,
        [string]$str3)
    $input = $str1

    if (-Not (IsEmptyString($str2))) {
        $input += " $str2"
    }

    if (-Not (IsEmptyString($str3))) {
        $input += " $str3"
    }

    $str = Printx $input -fc 'black' -bc 'red'
    Write-Information $str -InformationAction Continue
}

Function LogInfo() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$str1,
        [string]$str2,
        [string]$str3)
    $input = $str1

    if (-Not (IsEmptyString($str2))) {
        $input += " $str2"
    }

    if (-Not (IsEmptyString($str3))) {
        $input += " $str3"
    }

    $str = Printx $input -fc 'blue'
    Write-Information $str -InformationAction Continue
}


Function IsEmptyString
{
    [CmdletBinding()]
    param (
        [string]$s
    )

    Write-Output $([string]::IsNullOrEmpty($s))
}

Export-ModuleMember -Function 'LogInfo'
Export-ModuleMember -Function 'LogError'
Export-ModuleMember -Function 'LogWarning'
Export-ModuleMember -Function 'LogBuildInfo'
Export-ModuleMember -Function 'LogProjectInfo'
Export-ModuleMember -Function 'IsEmptyString'