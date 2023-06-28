using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Options;

namespace JwtTokenClaimsConsumerAPI.Controllers;

using Microsoft.AspNetCore.Mvc;
/// <summary>
/// The jwt custom user from claims controller.
/// </summary>

[ApiController]
[Route("[controller]")]
public class JwtValidDataCenterUserClaimsController : ODataController
{
    private readonly ILogger<JwtValidDataCenterUserClaimsController> _logger;
    private readonly ValidUserEndpointOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="JwtCustomUserFromClaimsController"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="options">The options.</param>
    public JwtValidDataCenterUserClaimsController(ILogger<JwtValidDataCenterUserClaimsController> logger,
        IOptions<ValidUserEndpointOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    /// <summary>
    /// Gets the.
    /// </summary>
    /// <returns>An IActionResult.</returns>
    [HttpGet("")]
    [EnableQuery]
    public IActionResult Get()
    {
        var claims = HttpContext.User.Claims;
        var userJson = User.FindFirst(c => c.Type == _options.CustomClaimName)?.Value;
        if (userJson == null)
            return BadRequest();
        var user = JsonSerializer.Deserialize<CustomUser>(userJson);

        return Ok(user);

    }
}