using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace TCPserver
{
    class TokenManager
    {
        public string GenerateToken(string playfabId)
        {
            Console.WriteLine(playfabId);
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("eThWmZq4t7w!z%C&F)J@NcRfUjXn2r5u"));
            var token = new JwtSecurityToken(
                claims: new Claim[]
                {
           // new Claim(JwtRegisteredClaimNames.Iss, "12345"),
            new Claim(JwtRegisteredClaimNames.Jti, playfabId),
            //new Claim(JwtRegisteredClaimNames.Sub, "12345"),
            //new Claim("a", "12345"),
            new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToString(), ClaimValueTypes.Integer64),
                },
                //notBefore: new DateTimeOffset(DateTime.Now).DateTime,
                expires: new DateTimeOffset(DateTime.Now.AddMinutes(1)).DateTime,
                signingCredentials: new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512)
            );

            string jwt = new JwtSecurityTokenHandler().WriteToken(token);
            this.DecryptToken(jwt);
            return jwt;
            //var header = "{\"typ\":\"JWT\",\"alg\":\"HS256\"}";
            //var claims = "{\"playfabId\":\""+ playfabId+"\"}";

            //var b64header = Convert.ToBase64String(Encoding.UTF8.GetBytes(header))
            //    .Replace('+', '-')
            //    .Replace('/', '_')
            //    .Replace("=", "");
            //var b64claims = Convert.ToBase64String(Encoding.UTF8.GetBytes(claims))
            //    .Replace('+', '-')
            //    .Replace('/', '_')
            //    .Replace("=", "");

            //var payload = b64header + "." + b64claims;
            //Console.WriteLine("JWT without sig:    " + payload);

            //byte[] key = Encoding.UTF8.GetBytes("eThWmZq4t7w!z%C&F)J@NcRfUjXn2r5u");
            //byte[] message = Encoding.UTF8.GetBytes(payload);

            //string sig = Convert.ToBase64String(HashHMAC(key, message))
            //    .Replace('+', '-')
            //    .Replace('/', '_')
            //    .Replace("=", "");

            //Console.WriteLine("JWT with signature: " + payload + "." + sig);
            //DecryptToken(sig);
            //return payload;
        }

        private static byte[] HashHMAC(byte[] key, byte[] message)
        {
            var hash = new HMACSHA256(key);
            return hash.ComputeHash(message);
        }

        public SecurityToken DecryptToken(string token)
        {
            //token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IklhbiBab25qYSIsImlhdCI6MTUxNjIzOTAyMn0.A4ISCJ2ESJsmrTrRZIg_MOk067DMLDVHQnoLXrKR6aY";
            try
            {
                string secret = "eThWmZq4t7w!z%C&F)J@NcRfUjXn2r5u";
                var key = Encoding.ASCII.GetBytes(secret);
                var handler = new JwtSecurityTokenHandler();
                var validations = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false
                };
                var claims = handler.ValidateToken(token, validations, out var tokenSecure);
                return tokenSecure;
            }catch(Exception ex)
            {
                Console.WriteLine("Unauthorized");
                return null;
            }
        }
    }
}
