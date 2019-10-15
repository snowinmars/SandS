Function Copy-SshPublicKey() {
    [CmdletBinding()]
    param (
        [Parameter (Mandatory=$true)]
        $publicKeyFilePath
    )

}

Export-ModuleMember -Function 'Copy-SshPublicKey'

