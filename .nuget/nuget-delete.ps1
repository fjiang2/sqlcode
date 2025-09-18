# Usage: & ./nuget-delete.ps1

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

$env:Version=$versionNumber

$projects = @(
	"sqlcode",
	"sqlcode.sqlce",
	"sqlcode.sqlclient",
	"sqlcode.sqlite",
	"sqlcode.sqlremote"
) 

For ($i=0; $i -lt $projects.Length; $i++) {
   $pak = "$Env:USERPROFILE\.nuget\packages\$($projects[$i])\$Env:Version"
   if (Test-Path -Path $pak) 
   {
      Remove-Item -Recurse -Force  $pak
   }
   else {
      Write-Host "not found" $pak
   }
}


For ($i=0; $i -lt $projects.Length; $i++) {
   $pak = "C:\local\nuget\$($projects[$i]).$Env:Version.nupkg"
   if (Test-Path -Path $pak) 
   {
      Remove-Item -Recurse -Force  $pak
   }
   else {
      Write-Host "not found" $pak
   }
}
