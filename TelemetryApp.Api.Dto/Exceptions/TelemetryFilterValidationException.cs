namespace TelemetryApp.Api.Dto.Exceptions;

public class TelemetryFilterValidationException : Exception
{
    public TelemetryFilterValidationException(string[] values, string allowedCollectionName, string forbiddenCollectionName)
        : base($"{string.Join(", ", values)} {(values.Length == 1 ? "was" : "were")} both in {allowedCollectionName} and {forbiddenCollectionName}")
    {
    }
}