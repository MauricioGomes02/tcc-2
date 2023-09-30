using Newtonsoft.Json;
using System.Text;
using System.Web;
using Tcc2.Domain.Entities.ValueObjects;
using Tcc2.Domain.Interfaces.Infrastructure.Services;
using Tcc2.Infrastructure.Models.Outputs;

namespace Tcc2.Infrastructure.Services;

public class GeographicCoordinateService : IGeographicCoordinateService
{
    private readonly IHttpClientFactory _httpClientFactory;

    private const string SchemaHttp = "https";
    private const string HostHttp = "geocode.maps.co";
    private const string PathHttp = "search";

    public GeographicCoordinateService(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<GeographicCoordinate?> GetAsync(Address address, CancellationToken cancellationToken)
    {
        var request = BuildRequest(address);

        var client = _httpClientFactory.CreateClient();

        var responseMessage = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
        if (!responseMessage.IsSuccessStatusCode)
        {
            throw new HttpRequestException("Unable to obtain the geographic coordinates of the address provided");
        }

        var @object = await responseMessage.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var output = JsonConvert.DeserializeObject<IEnumerable<GeographicCoordinateOutput>>(@object);

        if (output is null || !output.Any())
        {
            return null;
        }

        var geographicCoordinate = output.First();

        return new GeographicCoordinate(geographicCoordinate.Latitude, geographicCoordinate.Longitude);
    }

    private static string BuildQuery(IEnumerable<Tuple<string, string>> keyValues)
    {
        var stringBuilder = new StringBuilder();
        foreach (var keyValue in keyValues)
        {
            if (stringBuilder.Length > 0)
            {
                stringBuilder.Append('&');
            }
            stringBuilder.Append($"{keyValue.Item1}=");
            stringBuilder.Append(HttpUtility.UrlEncode(keyValue.Item2));
        }
        return stringBuilder.ToString();
    }

    private static Uri BuildUri(string query)
    {
        var uriBuilder = new UriBuilder();
        uriBuilder.Scheme = SchemaHttp;
        uriBuilder.Host = HostHttp;
        uriBuilder.Path = PathHttp;
        uriBuilder.Query = query;

        return uriBuilder.Uri;
    }

    private static IEnumerable<Tuple<string, string>> BuildQueryObjects(
        string country,
        string state,
        string city,
        string street)
    {
        return new List<Tuple<string, string>>
        {
            new Tuple<string, string>("country", country),
            new Tuple<string, string>("state", state),
            new Tuple<string, string>("city", city),
            new Tuple<string, string>("street", street)
        };
    }

    private static HttpRequestMessage BuildRequest(Address address)
    {
        var queryObjects = BuildQueryObjects(
            address.Country,
            address.State,
            address.City,
            $"{address.Number} {address.Street}");

        var queryString = BuildQuery(queryObjects);
        var uri = BuildUri(queryString);
        return new HttpRequestMessage(HttpMethod.Get, uri);
    }
}
