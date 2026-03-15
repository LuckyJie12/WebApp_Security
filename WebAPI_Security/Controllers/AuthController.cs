using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace WebAPI_Security.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public AuthController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        [HttpPost]
        public IActionResult Authenticate([FromBody] Credential credential)
        {
            if (credential.Username == "admin" && credential.Password == "123")
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, credential.Username),
                    new(ClaimTypes.Email, "snow.hero@qq.com"),
                    new("Department", "HR"),
                    new("Admin", "true"),
                    new("Manager", "true"),
                    new("DepartmentDate","2025-3-13")
                };

                var expiresAt = DateTime.UtcNow.AddMinutes(10);

                return Ok(new
                {
                    access_token = CreateToken(claims, expiresAt),
                    expires_at = expiresAt,
                });
            }
            ModelState.AddModelError("未经授权", "用户名或密码无效");
            var problemDetails = new ValidationProblemDetails(ModelState)
            {
                Status = StatusCodes.Status401Unauthorized,
                Title = "未经授权",
                Detail = "用户名或密码无效"
            };
            return Unauthorized(problemDetails);
        }
        private string CreateToken(List<Claim> claims, DateTime expiresAt)
        {
            var claimsDic = new Dictionary<string, object>();
            if (claims is not null && claims.Count > 0)
            {
                foreach (var claim in claims)
                {
                    if (claim.Type == ClaimTypes.Name)
                    {
                        claimsDic.Add(claim.Type, claim.Value);
                    }
                }
            }
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["SecretKey"] ?? string.Empty)), SecurityAlgorithms.HmacSha256Signature),
                Claims = claimsDic,
                Subject = new ClaimsIdentity(claims),
                NotBefore = DateTime.UtcNow,
                Expires = expiresAt,
            };
            var tokenHandler = new JsonWebTokenHandler();
            return tokenHandler.CreateToken(tokenDescriptor);
        }
    }
    public class Credential
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
