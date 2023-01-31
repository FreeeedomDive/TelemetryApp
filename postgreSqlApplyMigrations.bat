set ASPNETCORE_ENVIRONMENT=Production
dotnet tool install --global dotnet-ef
dotnet ef database update --project TelemetryApp.Core --startup-project TelemetryApp.Api
pause