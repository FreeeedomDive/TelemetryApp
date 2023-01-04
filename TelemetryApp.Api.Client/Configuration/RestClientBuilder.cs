using RestSharp;

namespace TelemetryApp.Api.Client.Configuration;

internal static class RestClientBuilder
{
    public static RestClient BuildRestClient(string url, bool validateSsl = true)
    {
        var restClientOptions = new RestClientOptions
        {
            BaseUrl = new Uri(url)
        };
        if (!validateSsl)
        {
            restClientOptions.RemoteCertificateValidationCallback = (_, _, _, _) => true;
        }
        return new RestClient(restClientOptions);
    }
}