using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using Microsoft.Extensions.Options;

namespace JwtTokenClaimsConsumerAPI.Controllers;

using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class JwtCustomUserFromClaimsController : ODataController
{
    private readonly ILogger<JwtCustomUserFromClaimsController> _logger;
    private readonly AuthorizedAccountEndpointOptions _options;

    public JwtCustomUserFromClaimsController(ILogger<JwtCustomUserFromClaimsController> logger,
        IOptions<AuthorizedAccountEndpointOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }
    
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