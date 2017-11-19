Param(
	[switch]$pushLocal,
	[switch]$pushNuget
)

if (Test-Path -Path nuget-powershell) 
{
	rmdir nuget-powershell -Recurse
}
if (Test-Path -Path nuget-cmdline) 
{
	rmdir nuget-cmdline -Recurse
}

rm .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.Http\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.Queue\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.Cache\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\debug\*.nupkg
rm .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\debug\*.nupkg

msbuild

if ($pushLocal)
{
	cp .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AzureFromTheTrenches.Commanding\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AzureFromTheTrenches.Commanding.Http\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository
	cp .\Source\AzureFromTheTrenches.Commanding.Queue\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\debug\*.nupkg \MicroserviceAnalyticPackageRepository	
}

if ($pushNuget)
{
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Http\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Queue\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\debug\*.nupkg --source nuget.org
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\debug\*.nupkg --source nuget.org
}
