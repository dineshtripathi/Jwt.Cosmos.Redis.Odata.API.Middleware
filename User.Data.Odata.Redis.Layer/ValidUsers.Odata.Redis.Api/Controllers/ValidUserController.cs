using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Routing.Controllers;
using ValidUsers.Odata.Redis.Api.Model;

namespace ValidUsers.Odata.Redis.Api.Controllers
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
        [EnableQuery]
        public IEnumerable<ValidUser> GetAllValidUsers(Guid dataCentreId)
        {
            return Enumerable.Range(1, 5).Select(index => new ValidUser
            {
                JoiningDate = DateTime.UtcNow.AddDays(12),
                UserId = Random.Shared.Next(-20, 55).ToString(),
                DepartmentId = Departments[Random.Shared.Next(Departments.Length)],
                Email = Departments[Random.Shared.Next(Departments.Length)],
                EmploymentType="",
                LocationId="",
                UserCode="",
                UserContactNumber="",
                UserFirstName="",
                UserLastName="",
                AccountExpiryDate=DateTime.UtcNow,

            })
            .ToArray();
        }
    }
}