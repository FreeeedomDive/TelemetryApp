using Newtonsoft.Json;
using RestSharp;

namespace TelemetryApp.Api.Client.Extensions;

internal static class RestResponseExtensions
{
    public static void ThrowIfNotSuccessful(this RestResponse restResponse)
    {
        if (restResponse.IsSuccessful) return;

        if (restResponse.Content == null)
        {
            throw new Exception("Content is null");
        }

        var exception = JsonConvert.DeserializeObject<Exception>(restResponse.Content);
        throw exception ?? new Exception("Unknown API error");
    }

    public static T TryDeserialize<T>(this RestResponse restResponse)
    {
        restResponse.ThrowIfNotSuccessful();
        if (restResponse.Content == null)
        {
            throw new Exception("Content is null");
        }

        try
        {
            var response = JsonConvert.DeserializeObject<T>(restResponse.Content);
            if (response == null)
            {
                throw new Exception($"Can not deserialize response as {typeof(T).Name}");
            }

            return response;
        }
        catch
        {
            throw new Exception($"Can not deserialize response as {typeof(T).Name}");
        }
    }
}