using Newtonsoft.Json;

namespace ValidUsers.Odata.Redis.Api.Model
{
    /// <summary>
    /// The valid user.
    /// </summary>
    public class ValidUser
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        [JsonProperty(PropertyName = "id")]
        public required string UserId { get; set; }
        /// <summary>
        /// Gets or sets the account expiry date.
        /// </summary>
        public DateTime AccountExpiryDate { get; set; } = DateTime.Now;
        /// <summary>
        /// Gets or sets the department id.
        /// </summary>
        public required string DepartmentId { get; set; }
        /// <summary>
        /// Gets or sets the location id.
        /// </summary>
        public required string LocationId { get; set; }
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public required string Email { get; set; }
        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        public required string UserCode { get; set; }
        /// <summary>
        /// Gets or sets the user first name.
        /// </summary>
        public required string UserFirstName { get; set; }
        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        public required string UserLastName { get; set; }
        /// <summary>
        /// Gets or sets the user contact number.
        /// </summary>
        public required string UserContactNumber { get; set; }
        /// <summary>
        /// Gets or sets the employment type.
        /// </summary>
        public required string EmploymentType { get; set; }
        /// <summary>
        /// Gets or sets the joining date.
        /// </summary>
        public DateTime JoiningDate { get; set; }
    }

}