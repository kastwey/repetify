# Generates a 256 bits random key (32 bytes).
$byteCount = 32
$randomBytes = New-Object 'System.Byte[]' ($byteCount)

$rng = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$rng.GetBytes($randomBytes)
$rng.Dispose()

$base64Key = [Convert]::ToBase64String($randomBytes)

$base64Key | Set-Clipboard
Write-Host "Your new Signing key has been copied to your clipboard:"
Write-Host $base64Key
