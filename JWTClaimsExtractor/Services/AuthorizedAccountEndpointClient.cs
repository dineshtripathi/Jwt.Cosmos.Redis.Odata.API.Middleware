using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace JWTClaimsExtractor.Services;

public class AuthorizedAccountEndpointClient
{
    private readonly string? _endpointRoute;
    private readonly HttpClient? _httpClient;

    public AuthorizedAccountEndpointClient(IOptions<AuthorizedAccountEndpointOptions> options)
    {
        if (string.IsNullOrEmpty(_endpointRoute) || string.IsNullOrWhiteSpace(options.Value.BaseUri) ||
            string.IsNullOrWhiteSpace(options.Value.ApiKey)) return;
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri(options.Value.BaseUri)
        };
        _httpClient.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", options.Value.ApiKey);
        _endpointRoute = options.Value.EndpointRoute;
    }

    public async Task<IEnumerable<AuthorizedAccount>?> GetAuthorizedAccounts(string? orgId,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_endpointRoute) || string.IsNullOrWhiteSpace(orgId))
        {
            return Enumerable.Empty<AuthorizedAccount>();
        }

        if (_httpClient == null)
            return Enumerable.Empty<AuthorizedAccount>();
        var response = await _httpClient.GetAsync($"{_endpointRoute.Replace("{0}", orgId)}", cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception($"Unable to get authorized accounts: {response.ReasonPhrase}");


        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var jObject = JObject.Parse(json);

        var items = jObject["response"]?["items"] as JArray;

        return items?.ToObject<List<AuthorizedAccount>>();
    }
}