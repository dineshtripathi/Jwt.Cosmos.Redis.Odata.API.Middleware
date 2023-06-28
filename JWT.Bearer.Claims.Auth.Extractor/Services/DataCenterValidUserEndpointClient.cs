using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace JWTClaimsExtractor.Services;
/// <summary>
/// The authorized account endpoint client.
/// </summary>

public class DataCenterValidUserEndpointClient
{
    private readonly string? _endpointRoute;
    private readonly HttpClient? _httpClient;

    /// <summary>
    /// Initializes a new instance of the <see cref="DataCenterValidUserEndpointClient"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    public DataCenterValidUserEndpointClient(IOptions<ValidUserEndpointOptions> options)
    {
        if (string.IsNullOrEmpty(options.Value.EndpointRoute) || string.IsNullOrWhiteSpace(options.Value.ApiGatewayEndpoint) ||
            string.IsNullOrWhiteSpace(options.Value.ApiKey)) return;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.Value.ApiGatewayEndpoint)
        };
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Value.ApiKey);
        _endpointRoute = options.Value.EndpointRoute;
    }

    /// <summary>
    /// Gets the authorized accounts.
    /// </summary>
    /// <param name="orgId">The org id.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>A Task.</returns>
    public async Task<IEnumerable<Object>?> GetDataCentreValidUser(string? orgId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_endpointRoute) || string.IsNullOrWhiteSpace(orgId))
            return Enumerable.Empty<Object>();

        if (_httpClient == null)
            return Enumerable.Empty<Object>();
        var response = await _httpClient.GetAsync($"{_endpointRoute.Replace("{0}", orgId)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Unable to get valid users: {response.ReasonPhrase}");


        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var jObject = JObject.Parse(json);

        var items = jObject["response"]?["items"] as JArray;

        return items?.ToObject<List<Object>>();
    }
}