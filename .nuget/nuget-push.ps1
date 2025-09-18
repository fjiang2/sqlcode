# Usage: & ./nuget-push.ps1 <version>

param(
    [string]$Version=$Env:BUILDVER 	#BUILDVER X.Y.Z.A
)

$versionPattern = "\d+(\.\d+){1,2}"
$match = $Version | Select-String -Pattern $versionPattern
if ($match) {
    $versionNumber = $match.Matches.Value
    Write-Host "Version: $versionNumber"
} else {
    Write-Host "No version number found."
}


cd C:\devel\GitHub\sqlcode\.nuget

$env:NugetReleasePath="C:\local\nuget"
$env:Version=$versionNumber

$projects = @(
	"sqlcode",
	"sqlcode.sqlce",
	"sqlcode.sqlclient",
	"sqlcode.sqlite",
	"sqlcode.sqlremote"
) 

For ($i=0; $i -lt $projects.Length; $i++) {
   $pak = "$Env:NugetReleasePath\$($projects[$i]).$Env:Version.nupkg"
   if (Test-Path -Path $pak) 
   {
      nuget push $pak -Source https://api.nuget.org/v3/index.json
   }
   else 
   {
      Write-Host "not found" $pak
   }
}

