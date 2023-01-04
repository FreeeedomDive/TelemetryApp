set ASPNETCORE_ENVIRONMENT=Production
dotnet ef database update --project TelemetryApp.Core --startup-project TelemetryApp.Api
pause