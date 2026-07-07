param(
    [Parameter(Mandatory = $true)]
    [string]$PrivateKeyPath,

    [string]$PublicKeyPath,

    [switch]$UpdateSource
)

$ErrorActionPreference = 'Stop'

$resolvedPrivatePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PrivateKeyPath)
$privateDirectory = Split-Path -Parent $resolvedPrivatePath
if (-not (Test-Path -LiteralPath $privateDirectory)) {
    New-Item -ItemType Directory -Path $privateDirectory | Out-Null
}

$rsa = [System.Security.Cryptography.RSACryptoServiceProvider]::new(3072)
$rsa.PersistKeyInCsp = $false
try {
    $privateXml = $rsa.ToXmlString($true)
    $publicXml = $rsa.ToXmlString($false)

    [System.IO.File]::WriteAllText($resolvedPrivatePath, $privateXml, [System.Text.Encoding]::UTF8)

    if ($PublicKeyPath) {
        $resolvedPublicPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PublicKeyPath)
        $publicDirectory = Split-Path -Parent $resolvedPublicPath
        if (-not (Test-Path -LiteralPath $publicDirectory)) {
            New-Item -ItemType Directory -Path $publicDirectory | Out-Null
        }

        [System.IO.File]::WriteAllText($resolvedPublicPath, $publicXml, [System.Text.Encoding]::UTF8)
    }

    if ($UpdateSource) {
        $sourcePath = Join-Path $PSScriptRoot '..\RcloneMountGUI\UpdateSecurity.cs'
        $source = [System.IO.File]::ReadAllText($sourcePath)
        $escapedPublicXml = $publicXml.Replace('\', '\\').Replace('"', '\"')
        $pattern = 'public const string UpdatePublicKeyXml = ".*?";'
        $replacement = 'public const string UpdatePublicKeyXml = "' + $escapedPublicXml + '";'
        $updated = [System.Text.RegularExpressions.Regex]::Replace($source, $pattern, $replacement)
        [System.IO.File]::WriteAllText($sourcePath, $updated, [System.Text.Encoding]::UTF8)
    }

    Write-Host "Private key written to: $resolvedPrivatePath"
    if ($PublicKeyPath) {
        Write-Host "Public key written to: $resolvedPublicPath"
    } else {
        Write-Host "Public key:"
        Write-Host $publicXml
    }
} finally {
    $rsa.Dispose()
}
