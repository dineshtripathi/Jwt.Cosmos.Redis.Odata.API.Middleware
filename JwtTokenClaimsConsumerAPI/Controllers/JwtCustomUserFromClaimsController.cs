using System.Text.Json;
using JWTClaimsExtractor.Claims;
using JWTClaimsExtractor.ConfigSection;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace JwtTokenClaimsConsumerAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class JwtCustomUserFromClaimsController : ControllerBase
{
    private readonly ILogger<JwtCustomUserFromClaimsController> _logger;
    private readonly AuthorizedAccountEndpointOptions _options;

    public JwtCustomUserFromClaimsController(ILogger<JwtCustomUserFromClaimsController> logger,
        IOptions<AuthorizedAccountEndpointOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    [HttpGet("GetAllClaimsUser")]
    public OkObjectResult Get()
    {
        var claims = HttpContext.User.Claims;
        var userJson = User.FindFirst(c => c.Type == _options.CustomClaimName)?.Value;
        var user = JsonSerializer.Deserialize<CustomUser>(userJson);

        return Ok(user);
    }
}

[ApiController]
public class ErrorController : ControllerBase
{
    [Route("/error")]
    public IActionResult Error()
    {
        var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
        var exception = context?.Error; // Your exception

        if (exception is UnauthorizedAccessException)
        {
            Response.StatusCode = StatusCodes.Status401Unauthorized;
            return Problem("Unauthorized access", title: exception.Message);
        }

        Response.StatusCode = StatusCodes.Status500InternalServerError;
        return Problem(context?.Error.StackTrace, // You may not want to expose the stack trace
            title: context?.Error.Message);
    }
}