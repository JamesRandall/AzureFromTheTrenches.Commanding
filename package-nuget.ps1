Param(
	[switch]$pushLocal,
	[switch]$pushNuget,
	[switch]$cleanup
)

if (Test-Path -Path nuget-powershell) 
{
	rmdir nuget-powershell -Recurse
}
if (Test-Path -Path nuget-cmdline) 
{
	rmdir nuget-cmdline -Recurse
}
rm *.nupkg

msbuild

if ($pushLocal)
{
	cp .\Source\AccidentalFish.Commanding\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Commanding.AzureStorage\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Commanding.Http\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AccidentalFish.Commanding.Queue\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository	
}

if ($pushNuget)
{
	dotnet nuget push .\Source\AccidentalFish.Commanding\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AccidentalFish.Commanding.AzureStorage\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AccidentalFish.Commanding.Http\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AccidentalFish.Commanding.Queue\bin\debug\*.nupkg --source nuget.org
}

if ($cleanup)
{
	rmdir nuget-powershell -Recurse
	rm *.nupkg
}
