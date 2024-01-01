This application aims to identify the 5 different built in ASP.NET core ways of accessing configuration settings.

1. appsettings.json
	1.1 appsettings.Development.json (this runs only if you run your app in development mode. The answer is not when running in visual studio...
	instead, it means when you expand properties folder >> launchSettings.json)
	1.2 We look in appsettings.Development.json before we look in appsettings.json for a particular configuration value 
		(in other words, development overrides whatever values are in appsettings.json)
	1.3 The override only happens if in launchSettings.json we have ASPNETCORE_ENVIRONMENT": "Development"
2. secrets
3. azure.appsettings
	3.1 azure keyvault (super locked down)
4. appsettings.Development.json
5. 




Side Note: Command line arguments are higher in the foodchain than appsettings.json settings
...environment variables as well.

