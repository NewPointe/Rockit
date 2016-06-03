param (
    [Parameter(Mandatory=$true)][string]$Name
 )
$scriptPath = Split-Path -parent $MyInvocation.MyCommand.Definition

Set-Location $scriptPath

$prefix="org.newpointe"
$templatename="$prefix.Template"
$newprojectname="$prefix.$Name"
$oldguid=[guid]::Empty.tostring()
$newguid=[guid]::newguid().tostring()

Copy-Item $templatename $newprojectname -Recurse

Set-Location $newprojectname

Move-Item "$templatename.csproj" -Destination "$newprojectname.csproj"

(Get-Content "$newprojectname.csproj") -replace $templatename,$newprojectname -replace $oldguid,$newguid | Set-Content "$newprojectname.csproj"
(Get-Content "Properties\AssemblyInfo.cs") -replace $templatename,$newprojectname -replace $oldguid,$newguid | Set-Content "Properties\AssemblyInfo.cs"

Read-Host "Done"