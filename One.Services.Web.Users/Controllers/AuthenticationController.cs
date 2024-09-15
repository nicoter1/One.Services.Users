using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using One.Models.Users.Security;

namespace One.Services.Web.Users.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController(IConfiguration configuration, ILogger<AuthenticationController> logger) : ControllerBase
    {
        [HttpPost("login")]
        public IActionResult Login([FromBody] Client model)
        {
            var jwtSettings = configuration.GetSection("Jwt").Get<JwtSettings>();
            // Validate user credentials (this is just a sample; use a real validation)
            if (jwtSettings != null && (jwtSettings.Clients.Any(u =>
                    u.Username.Equals(model.Username, StringComparison.InvariantCultureIgnoreCase) &&
                    u.Password == model.Password)))
            {
                logger.LogInformation($"login successful at {DateTime.UtcNow}");


                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(jwtSettings.Key);
                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new Claim[]
                    {
                        new Claim(ClaimTypes.Name, model.Username)
                    }),
                    Expires = DateTime.UtcNow.AddHours(1),
                    Issuer = jwtSettings.Issuer,
                    Audience = jwtSettings.Audience,
                    SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key),
                        SecurityAlgorithms.HmacSha256Signature)
                };
                var token = tokenHandler.CreateToken(tokenDescriptor);
                var tokenString = tokenHandler.WriteToken(token);

                return Ok(new { Token = tokenString });
            }
            else
            {
                logger.LogInformation($"login unsuccessful at {DateTime.UtcNow}");
                return Unauthorized();
            }
        }
    }
}
