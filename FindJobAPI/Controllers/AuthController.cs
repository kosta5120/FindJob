using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FindJobAPI.Data;
using FindJobAPI.DTOs;

namespace FindJobAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly FindJobContext _db;
    private readonly IConfiguration _config;

    public AuthController(FindJobContext db, IConfiguration config)
    {
        _db = db;
        _config = config;
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResultDto>> Login([FromBody] LoginDto dto)
    {
        var admin = await _db.Administrators
            .FirstOrDefaultAsync(a => a.UserName == dto.Username);

        if (admin == null || admin.Password != dto.Password)
            return Unauthorized(new { message = "Invalid credentials" });

        var token = GenerateToken(admin.UserName);
        return Ok(new LoginResultDto { Token = token, Username = admin.UserName });
    }

    private string GenerateToken(string username)
    {
        var jwtKey = _config["Jwt:Key"] ?? "FindJobDefaultSecretKey_ChangeInProduction_32chars";
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "FindJobAPI",
            audience: _config["Jwt:Audience"] ?? "FindJobClient",
            claims: [new Claim(ClaimTypes.Name, username), new Claim(ClaimTypes.Role, "Admin")],
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
