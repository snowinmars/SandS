$ErrorActionPreference = "Stop"
[console]::OutputEncoding = [Text.Encoding]::Utf8

$root = $PSScriptRoot

Import-Module (Join-Path $root '_build.psm1') -Force
Import-Module (Join-Path $root '_console.psm1') -Force
Import-Module (Join-Path $root '_git.psm1') -Force
Import-Module (Join-Path $root '_localUri.psm1') -Force
