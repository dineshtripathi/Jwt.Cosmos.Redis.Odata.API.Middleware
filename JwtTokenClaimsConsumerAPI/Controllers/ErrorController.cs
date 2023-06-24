using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace JwtTokenClaimsConsumerAPI.Controllers;
/// <summary>
/// The error controller.
/// </summary>

[ApiController]
public class ErrorController : ControllerBase
{
    //[Route("/error")]
    //public IActionResult Error()
    //{
    //    var context = HttpContext.Features.Get<IExceptionHandlerFeature>();
    //    var exception = context?.Error; // Your exception

    //    // Customize the error response based on the exception type or any other logic
    //    if (exception is UnauthorizedAccessException)
    //    {
    //        Response.StatusCode = StatusCodes.Status401Unauthorized;
    //        return Problem("Unauthorized access", title: exception.Message);
    //    }

    //    Response.StatusCode = StatusCodes.Status500InternalServerError;
    //    return Problem(context?.Error.StackTrace, title: context?.Error.Message);
    //}
}