param(
    [Parameter(Mandatory = $true)]
    [string]$ExePath,

    [Parameter(Mandatory = $true)]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [string]$PrivateKeyPath,

    [string]$OutputPath
)

$ErrorActionPreference = 'Stop'

$resolvedExePath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($ExePath)
$resolvedPrivateKeyPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($PrivateKeyPath)

if (-not $OutputPath) {
    $OutputPath = Join-Path (Split-Path -Parent $resolvedExePath) 'RcloneMountGUI.update.json'
}
$resolvedOutputPath = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($OutputPath)

$assetName = [System.IO.Path]::GetFileName($resolvedExePath)
if ($assetName -ne 'RcloneMountGUI.exe') {
    throw "Expected executable asset name RcloneMountGUI.exe, got $assetName."
}

$hash = (Get-FileHash -LiteralPath $resolvedExePath -Algorithm SHA256).Hash.ToLowerInvariant()
$normalizedVersion = $Version.Trim().TrimStart('v', 'V')
$payload = "$normalizedVersion`n$assetName`n$hash`n"

$rsa = [System.Security.Cryptography.RSACryptoServiceProvider]::new()
$rsa.PersistKeyInCsp = $false
try {
    $privateXml = [System.IO.File]::ReadAllText($resolvedPrivateKeyPath)
    $rsa.FromXmlString($privateXml)
    $payloadBytes = [System.Text.Encoding]::UTF8.GetBytes($payload)
    $signatureBytes = $rsa.SignData($payloadBytes, [System.Security.Cryptography.CryptoConfig]::MapNameToOID('SHA256'))
    $signature = [Convert]::ToBase64String($signatureBytes)

    $manifest = [ordered]@{
        version = $normalizedVersion
        assetName = $assetName
        sha256 = $hash
        signature = $signature
    }

    $json = $manifest | ConvertTo-Json -Depth 3
    [System.IO.File]::WriteAllText($resolvedOutputPath, $json, [System.Text.Encoding]::UTF8)
    Write-Host "Manifest written to: $resolvedOutputPath"
} finally {
    $rsa.Dispose()
}
