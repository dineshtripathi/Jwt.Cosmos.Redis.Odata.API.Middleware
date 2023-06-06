using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace JwtTokenGenerateSigningKey
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var signingKey = GenerateSigningKey();
            var token = GenerateToken(signingKey);
        }

        static string GenerateSigningKey()
        {
            var hmac = new HMACSHA256();
            var key = Convert.ToBase64String(hmac.Key);

            Console.WriteLine($"{key}");
            Console.WriteLine("----------------------");
            Console.WriteLine("----------------------");


            return key;
        }

        static string GenerateToken(string signingKey)
        {
            var key = signingKey;
            var issuer = "https://localhost:7291";
            var audience = "JwtCustomUserFromClaimsAPI";


            var claims = new[]
            {
                new Claim("correlationId", "dec8a02a-3adb-4681-84c3-8d98ef9f16d3"),
                new Claim("email", "sse-engineering+direct@somoglobal.com"),
                new Claim("sub", "0a1591ce-e08e-4567-b42b-0fd4ff284e97"),
                new Claim("given_name", "Direct"),
                new Claim("family_name", "Customer"),
                new Claim("orgId", "4c78b740-25a6-47f4-57fa-08daa13cd7b8"),
                new Claim("orgRole", "Admin"),
                new Claim("extension_AgreedTermsAndConditionsVersion", "2021-04-23"),
                new Claim("name", "sse-engineering+direct@somoglobal.com"),
                new Claim("IsTpiConsultancy", "false"),
                new Claim("orgName", "Somo Global"),
                new Claim("tfp", "B2C_1A_rpSusiOrSspr-Dev"),
                new Claim("nonce", "43aebb6c-33d5-43a5-b852-4f5c6c3539fa"),
            };




            // Create Security key  using private key above:
            // not that latest version of JWT using Microsoft namespace instead of System
            var securityKey = new Microsoft
                .IdentityModel.Tokens.SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            // Also note that securityKey length should be >256b
            // so you have to make sure that your private key has a proper length
            //
            var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials
                (securityKey, SecurityAlgorithms.HmacSha256Signature);

            //  Finally create a Token
            var header = new JwtHeader(credentials);
            header.Add("kid","dineshtripathitesttoken");
            //Some PayLoad that contain information about the  customer
            var payload = new JwtPayload
            {
                {"sub", "0a1591ce-e08e-4567-b42b-0fd4ff284e97"},
                {"correlationId", "dec8a02a-3adb-4681-84c3-8d98ef9f16d3"},
                {"email", "sse-engineering+direct@somoglobal.com"},
                {"given_name", "Direct"},
                {"family_name", "Customer"},
                {"orgId", "4c78b740-25a6-47f4-57fa-08daa13cd7b8"},
                {"orgRole", "Admin"},
                {"extension_AgreedTermsAndConditionsVersion", "2021-04-23"},
                {"name", "sse-engineering+direct@somoglobal.com"},
                {"IsTpiConsultancy", "false"},
                {"orgName", "Somo Global"},
                {"tfp", "B2C_1A_rpSusiOrSspr-Dev"},
                {"nonce", "43aebb6c-33d5-43a5-b852-4f5c6c3539fa"}
            };

            //
            var secToken = new JwtSecurityToken(header, payload);
            var handler = new JwtSecurityTokenHandler();

            // Token to String so you can use it in your client
            var tokenString = handler.WriteToken(secToken);

            Console.WriteLine(tokenString);
            Console.WriteLine("Consume Token");


            // And finally when  you received token from client
            // you can  either validate it or try to  read
            var token = handler.ReadJwtToken(tokenString);

            Console.WriteLine(token.Payload.First().Value);

            Console.ReadLine();
            //var token = new JwtSecurityToken(issuer, audience, claims, DateTime.Now, DateTime.Now.AddHours(3), header, payload);

            //var tokenHandler = new JwtSecurityTokenHandler();
            //var tokenString = tokenHandler.WriteToken(token);

            //Console.WriteLine(tokenString);
            return tokenString;


        }
    }
}