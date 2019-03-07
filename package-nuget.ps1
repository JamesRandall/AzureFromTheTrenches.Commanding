Param(
	[switch]$pushLocal,
	[switch]$pushNuget,
	[switch]$disableBuild,
	[switch]$cleardown
)

if (Test-Path -Path nuget-powershell) 
{
	rmdir nuget-powershell -Recurse
}
if (Test-Path -Path nuget-cmdline) 
{
	rmdir nuget-cmdline -Recurse
}

if ($cleardown -Or -not $disableBuild)
{
	rm .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.AzureEventHub\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.Http\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.Queue\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.Cache\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection\bin\release\*.nupkg
	rm .\Source\AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions\bin\release\*.nupkg
}

if (-not $disableBuild)
{
	dotnet build .\AzureFromTheTrenches.Commanding.sln --configuration release
}

if ($pushLocal)
{
	cp .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding.AzureEventHub\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding.Http\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding.Queue\bin\release\*.nupkg \wip\functionMonkeyNuget	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache\bin\release\*.nupkg \wip\functionMonkeyNuget	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\release\*.nupkg \wip\functionMonkeyNuget	
	cp .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\release\*.nupkg \wip\functionMonkeyNuget	
	cp .\Source\AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection\bin\release\*.nupkg \wip\functionMonkeyNuget
	cp .\Source\AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions\bin\release\*.nupkg \wip\functionMonkeyNuget
}

if ($pushNuget)
{
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Abstractions\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.AzureStorage\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.AzureEventHub\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Http\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Queue\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache.MemoryCache\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.Cache.Redis\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.MicrosoftDependencyInjection\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
	dotnet nuget push .\Source\AzureFromTheTrenches.Commanding.MicrosoftLoggingExtensions\bin\release\*.nupkg --source https://www.nuget.org/api/v2/package
}
