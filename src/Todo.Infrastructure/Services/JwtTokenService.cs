using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using ToDoApp.Application.Interfaces;

namespace ToDoApp.Infrastructure.Services;

public class JwtTokenService(IConfiguration configuration) : IJwtTokenService
{
    public string CreateToken(Guid userId, string name, string email)
    {
        var issuer = configuration["Jwt:Issuer"]
            ?? throw new InvalidOperationException("A configuração Jwt:Issuer não foi definida.");
        var audience = configuration["Jwt:Audience"]
            ?? throw new InvalidOperationException("A configuração Jwt:Audience não foi definida.");
        var key = configuration["Jwt:Key"]
            ?? throw new InvalidOperationException("A configuração Jwt:Key não foi definida.");

        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Name, name),
            new Claim(JwtRegisteredClaimNames.Email, email)
        };

        var token = new JwtSecurityToken(
            issuer,
            audience,
            claims,
            expires: DateTime.UtcNow.AddHours(2),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
