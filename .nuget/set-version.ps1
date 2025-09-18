# Usage
# ./set-version.ps1 1.0.1

param(
    [string]$Version
)
[Environment]::SetEnvironmentVariable("NuGetVersion", $Version, "User")

$env:NuGetVersion=$Version

ls env:NuGetVersion