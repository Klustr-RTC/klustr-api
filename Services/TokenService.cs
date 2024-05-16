using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Klustr_api.Dtos.User;
using Klustr_api.Interfaces;
using Microsoft.IdentityModel.Tokens;

namespace backend.Services
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;

        public TokenService(IConfiguration config)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JWT:SigningKey"]!));
        }
        public string CreateToken(Guid userId, string email, string username)
        {
            var claims = new List<Claim>{
                new Claim(JwtRegisteredClaimNames.Email, email),
                new Claim(JwtRegisteredClaimNames.GivenName, username),
                new Claim("userId", userId.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddDays(7),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task<GoogleUserInfo?> GetUserInfoFromAccessToken(string accessToken)
        {
            string baseUrl = "https://www.googleapis.com/oauth2/v3/userinfo";
            //Have your using statements within a try/catch block
            try
            {
                //We will now define your HttpClient with your first using statement which will use a IDisposable.
                using (HttpClient client = new HttpClient
                {
                    DefaultRequestHeaders = {
                        Authorization = new AuthenticationHeaderValue("Bearer", accessToken)
                    }
                })
                {
                    //In the next using statement you will initiate the Get Request, use the await keyword so it will execute the using statement in order.
                    //The HttpResponseMessage which contains status code, and data from response.
                    using (HttpResponseMessage res = await client.GetAsync(baseUrl))
                    {
                        //Then get the data or content from the response in the next using statement, then within it you will get the data, and convert it to a c# object.
                        using (HttpContent content = res.Content)
                        {
                            var data = await content.ReadAsStringAsync();
                            JsonDocument doc = JsonDocument.Parse(data);
                            JsonElement root = doc.RootElement;
                            var userInfo = new GoogleUserInfo
                            {
                                Sub = root.GetProperty("sub").GetString() ?? "",
                                Name = root.GetProperty("name").GetString() ?? "",
                                Email = root.GetProperty("email").GetString() ?? "",
                                EmailVerified = root.GetProperty("email_verified").GetBoolean(),
                                GivenName = root.GetProperty("given_name").GetString() ?? "",
                                FamilyName = root.GetProperty("family_name").GetString() ?? "",
                                Picture = root.GetProperty("picture").GetString() ?? ""
                            };
                            return userInfo;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Exception Hit------------");
                Console.WriteLine(exception);
                return null;
            }
        }
    }
}