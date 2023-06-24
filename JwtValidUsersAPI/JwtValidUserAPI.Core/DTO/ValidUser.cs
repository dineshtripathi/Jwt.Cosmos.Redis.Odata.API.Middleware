namespace JwtValidUserAPI.Core.DTO
{
    /// <summary>
    /// The valid user.
    /// </summary>
    public class ValidUser
    {
        /// <summary>
        /// Gets or sets the user id.
        /// </summary>
        public string userId { get; set; } = string.Empty;

        /// <summary>
        /// Gets or sets the account expiry date.
        /// </summary>
        public DateTime accountExpiryDate { get; set; } = DateTime.Now;

        /// <summary>
        /// Gets or sets the department.
        /// </summary>
        public string department { get; set; } = string.Empty;
        /// <summary>
        /// Gets or sets the email.
        /// </summary>
        public string email { get; set; }
        /// <summary>
        /// Gets or sets the user code.
        /// </summary>
        public string userCode { get; set; }
        /// <summary>
        /// Gets or sets the user first name.
        /// </summary>
        public string userFirstName { get; set; }
        /// <summary>
        /// Gets or sets the user last name.
        /// </summary>
        public string userLastName { get; set; }
        /// <summary>
        /// Gets or sets the user contact number.
        /// </summary>
        public string  userContactNumber { get; set; }
        /// <summary>
        /// Gets or sets the employment type.
        /// </summary>
        public  string  employmentType{ get; set; }
        /// <summary>
        /// Gets or sets the joining date.
        /// </summary>
        public DateTime joiningDate { get;set; }
    }
}