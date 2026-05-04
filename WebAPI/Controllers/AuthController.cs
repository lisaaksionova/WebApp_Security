using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace WebAPI.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController(IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential.Username == "admin" && credential.Password == "password")
        {
            // creating context
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, credential.Username),
                new Claim(ClaimTypes.Email, "admin@admin.com"),
                new Claim("Department", "HR"),
                new Claim("Admin", "true"),
                new Claim("Manager", "true"),
                new Claim("EmploymentDate", "2025-05-01")
            };
            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            return Ok(new
            {
                access_token = CreateToken(claims, expiresAt),
                expires_at = expiresAt
            });
        }
        
        ModelState.AddModelError("Unauthorized", "Invalid username or password");
        var problemDetails = new ProblemDetails
        {
            Title = "Unauthorized",
            Status = StatusCodes.Status401Unauthorized,
            Detail = "Invalid username or password",
        };
        return Unauthorized(problemDetails);
    }

    private string CreateToken(List<Claim>? claims, DateTime expiresAt)
    {
        var claimsDict = new Dictionary<string, object>();
        if (claimsDict == null) throw new ArgumentNullException(nameof(claimsDict));
        
        if (claims is not null && claims.Any())
        {
            foreach (var claim in claims)
            {
               claimsDict.Add(claim.Type, claim.Value);
            }
        }
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(configuration["SecretKey"] ?? string.Empty)),
                SecurityAlgorithms.HmacSha256Signature),
            Claims = claimsDict,
            Expires = expiresAt,
            NotBefore = DateTime.Now,
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