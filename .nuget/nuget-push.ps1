# Usage: & ./nuget-push.ps1 <version>
# Example: & ./nuget-push.ps1 1.2.0

param(
	[Parameter(Mandatory=$true, Position=0)]
    [string]$Version
)

$env:NugetReleasePath="C:\local\nuget"
$env:Version=$Version

$projects = @(
	"sqlcode",
	"sqlcode.sqlce",
	"sqlcode.sqlclient",
	"sqlcode.sqlite",
	"sqlcode.sqlremote"
) 

# cd C:\devel\GitHub\sqlcode\.nuget
# For ($i=0; $i -lt $projects.Length; $i++) {
#   nuget pack "$($projects[$i]).nuspec"
#}

For ($i=0; $i -lt $projects.Length; $i++) {
   $pak = "$Env:NugetReleasePath\$($projects[$i]).$Env:Version.nupkg"
   if (Test-Path -Path $pak) {
      nuget push $pak -Source https://api.nuget.org/v3/index.json
   }
   else {
      Write-Host "not found" $pak
   }
}
