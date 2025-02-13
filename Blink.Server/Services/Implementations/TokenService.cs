using Blink.Models;
using Blink.Service.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Blink.Server.Services.Implementations
{
    public class TokenService : ITokenService
    {
        private readonly IConfiguration _config;
        private readonly SymmetricSecurityKey _key;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<TokenService> _logger;

        public TokenService(IConfiguration config, IHttpContextAccessor httpContextAccessor,
                            IHttpClientFactory httpClientFactory, UserManager<User> userManager, ILogger<TokenService> logger)
        {
            _config = config;
            _key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["JwtSettings:SecretKey"]));
            _httpContextAccessor = httpContextAccessor;
            _httpClientFactory = httpClientFactory;
            _userManager = userManager;
            _logger = logger;
        }

        public string CreateJWT(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Email),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
            };

            var creds = new SigningCredentials(_key, SecurityAlgorithms.HmacSha512Signature);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds,
                Issuer = _config["JWT:Issuer"],
                Audience = _config["JWT:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public void SetJWT(string jwt)
        {
            _httpContextAccessor.HttpContext?.Response.Cookies.Append("AuthToken", jwt, new CookieOptions
            {
                HttpOnly = true,       // Cookie inaccessible to JavaScript for security
                Secure = true,         // Only sent over HTTPS; set to false for local testing if needed
                SameSite = SameSiteMode.None,  // Cookie sent with cross-site requests
                Expires = DateTimeOffset.UtcNow.AddHours(1)  // Set expiration time for the JWT
            });
        }

        public string GetJWT()
        {
            return _httpContextAccessor.HttpContext.Request?.Cookies["AuthToken"];
        }

        public bool JWTIsExpired(string jwt)
        {
            if (string.IsNullOrEmpty(jwt))
            {
                return true;
            }
            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadJwtToken(jwt);

                var exp = jsonToken.Payload.Exp;

                if (exp == null || exp < DateTimeOffset.UtcNow.ToUnixTimeSeconds())
                {
                    return true;
                }

                return false;
            }
            catch (Exception)
            {
                return true;
            }
        }

        // public async Task<string> RefreshJwt()
        // {
        //     var client = _httpClientFactory.CreateClient();
        //     var jwt = GetJwt();

        //     if (TokenIsExpired(jwt))
        //     {
        //         var response = await client.PostAsync("api/auth/refresh-token", null);

        //         if (response.IsSuccessStatusCode)
        //         {
        //             var json = await response.Content.ReadAsStringAsync();
        //             var refreshedJwt = JsonSerializer.Deserialize<TokenResponse>(json);

        //             if (!string.IsNullOrEmpty(refreshedJwt?.Token))
        //             {
        //                 jwt = refreshedJwt.Token;
        //             }
        //             else
        //             {
        //                 throw new UnauthorizedAccessException("Unable to refresh token.");
        //             }
        //         }
        //     }
        //     return jwt;
        // }
    }
}
