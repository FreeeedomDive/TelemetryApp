using RestSharp;

namespace TelemetryApp.Utilities.Configuration;

internal static class RestClientBuilder
{
    private static RestClient BuildRestClient(string url, bool validateSsl = true)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(url),
        };
        if (!validateSsl)
        {
            restClientOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        }

        return new RestClient(restClientOptions);
    }

    public static RestClient BuildRestClient(string? customUrl = null)
    {
        return BuildRestClient(customUrl ?? "https://localhost:6651", false);
    }
}