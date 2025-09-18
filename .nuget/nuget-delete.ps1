# Usage: & ./nuget-delete.ps1

param(
	[Parameter(Mandatory=$true, Position=0)]
    [string]$Version=$Env:NuGetVersion 	#NuGetVersion X.Y.Z
)

$env:Version=$Version

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
