using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenGenerateSigningKey
{
    /// <summary>
    /// The program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Mains the.
        /// </summary>
        /// <param name="args">The args.</param>
        static void Main(string[] args)
        {
            var signingKey = GenerateSigningKey();
            var token = GenerateToken(signingKey);
        }

        /// <summary>
        /// Generates the signing key.
        /// </summary>
        /// <returns>A string.</returns>
        static string GenerateSigningKey()
        {
            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);

            Console.WriteLine($"{key}");
            Console.WriteLine("----------------------");
            Console.WriteLine("----------------------");


            return key;
        }

        /// <summary>
        /// Generates the token.
        /// </summary>
        /// <param name="signingKey">The signing key.</param>
        /// <returns>A string.</returns>
        static string GenerateToken(string signingKey)
        {
            var key = signingKey;
            var issuer = "https://localhost:7291";
            var audience = "JwtCustomUserFromClaimsAPI";


            var claims = new[]
            {
                new Claim("correlationId", Guid.NewGuid().ToString()),
                new Claim("email", "hitechdinesh@gmail.com"),
                new Claim("sub", Guid.NewGuid().ToString()),
                new Claim("given_name", "Dinesh"),
                new Claim("family_name", "Tripathi"),
                new Claim("usersCollectionsId", Guid.NewGuid().ToString()),
                new Claim("dataCenterRole", "Senior Engineer"),
                new Claim("extension_AgreedTermsAndConditionsVersion", "2021-04-23"),
                new Claim("name", "hitechdinesh@gmail.com"),
                new Claim("dataCenterName", "AAGS"),
                new Claim("tfp", "dinesh-github"),
                new Claim("nonce", Guid.NewGuid().ToString()),
            };




            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft
                .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials) {{"kid", "dineshtripathitesttoken"}};
            //Some PayLoad that contain information about the  customer

            var expirationTime = DateTime.UtcNow.AddHours(1);
            var unixExpirationTime = new DateTimeOffset(expirationTime).ToUnixTimeSeconds();

            var payload = new JwtPayload
            {
                {"sub", Guid.NewGuid().ToString()},
                {"correlationId", Guid.NewGuid().ToString()},
                {"email", "hitechdinesh@gmail.com"},
                {"given_name", "Dinesh"},
                {"family_name", "Tripathi"},
                {"usersCollectionsId", Guid.NewGuid().ToString()},
                {"dataCenterRole", "Senior Engineer"},
                {"extension_AgreedTermsAndConditionsVersion", "2021-04-23"},
                {"name", "hitechdinesh@gmail.com"},
                {"IsTpiConsultancy", "false"},
                {"dataCenterName", "AAGS"},
                {"tfp", "hitechdinesh@gmail.com"},
                {"nonce", Guid.NewGuid().ToString()},
                {"exp", unixExpirationTime} 


            };

            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);

            Console.WriteLine("\n\n------------------------------" );
            Console.WriteLine("Consume Token");

            var token = handler.ReadJwtToken(tokenString);

            Console.WriteLine(token.Payload.First().Value);

            Console.ReadLine();
            return tokenString;


        }
    }
}