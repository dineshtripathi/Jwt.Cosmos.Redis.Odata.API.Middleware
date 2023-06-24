using JwtValidUsersApi.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace JwtValidUsersApi.Controllers
{
    /// <summary>
    /// The Valid  User controller.
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class ValidUserController : ODataController
    {
        private static readonly string[] Departments = new[]
        {
        "LogicApp", "Docker", "ContainerApp", "AzureFunction", "EventGrid", "EventHub", "ServiceBus", "Storage", "ResourceGroups", "ExpressRoute"
    };

        private readonly ILogger<ValidUserController> _logger;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidUserController"/> class.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public ValidUserController(ILogger<ValidUserController> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Gets the.
        /// </summary>
        /// <returns>A list of ValidUsers.</returns>
        [HttpGet(Name = "GetAllValidUsers")]
        public IEnumerable<ValidUser> GetAllValidUsers(Guid dataCentreId)
        {
            return Enumerable.Range(1, 5).Select(index => new ValidUser
            {
                accountExpiryDate = DateTime.UtcNow.AddDays(12),
                userId = Random.Shared.Next(-20, 55).ToString(),
                department = Departments[Random.Shared.Next(Departments.Length)]
            })
            .ToArray();
        }
    }
}