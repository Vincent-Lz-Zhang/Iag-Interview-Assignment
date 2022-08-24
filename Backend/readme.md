# Instructions

The API app now runs at http://localhost:5001, and it only allows CORS for http://localhost:3000, if you want to specify the frontend domain, you can change the configuration in appsettings.json. Please note its value can not have a tailing forward slash '/'.

To test the custom error handler, you need to change ASPNETCORE_ENVIRONMENT (in launchSettings.json) to other values than 'Development', such as 'Staging' and 'Production'.