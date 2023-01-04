using RestSharp;

namespace Common.ClientBuilder;

public static class RestClientBuilder
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