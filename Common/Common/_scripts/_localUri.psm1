$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

$root = $PSScriptRoot

Import-Module (Join-Path $root '_console.psm1') -Force

Function JoinPath() {
    [CmdletBinding()]
    param(
        [Parameter (Mandatory=$true)]
        [string]$root,

        [string]$child1,
        [string]$child2,
        [switch]$shouldExists)

    $path = $root

    if (-Not (IsEmptyString($child1))) {
        $path = Join-Path $path $child1
    }

    if (-Not (IsEmptyString($child2))) {
        $path = Join-Path $path $child2
    }

    if ($shouldExists) {
        $path = (Resolve-Path $path).Path
    }

    Write-Output ([io.path]::GetFullPath($path))
}

Function Format-Buckets {
    [CmdletBinding()]
    param (
        [parameter(Mandatory=$true)]
        $items,
        [parameter(Mandatory=$true)]
        $itemsPerBucket
    )

    $bucketsCount = [math]::Ceiling($items.Count / $itemsPerBucket);

    if ($itemsPerBucket -eq 0) {
        throw "Wrong itemsPerBucket count : $itemsPerBucket"
    }

    $buckets = @()

    for ($i = 0; $i -lt $bucketsCount; $i++) {

        $bucket = $items | Select-Object -Skip ($i * $itemsPerBucket) -First $itemsPerBucket
        $buckets += , $bucket
    }

    Write-Output $buckets
}

Export-ModuleMember -Function 'JoinPath'
Export-ModuleMember -Function 'Format-Buckets'