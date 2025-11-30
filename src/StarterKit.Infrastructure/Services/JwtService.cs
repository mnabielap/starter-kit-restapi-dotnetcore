using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using StarterKit.Application.Common.Interfaces;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Infrastructure.Services;

public class JwtService : IJwtService
{
    private readonly IConfiguration _configuration;

    public JwtService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<(string Token, DateTime Expires)> GenerateTokenAsync(User user, TokenType type)
    {
        var secretKey = _configuration["Jwt:Secret"];
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new Claim("type", type.ToString())
        };

        if (type == TokenType.Access) 
        {
            claims.Add(new Claim("role", user.Role.ToString())); 
        }

        double expirationMinutes = type switch
        {
            TokenType.Access => double.Parse(_configuration["Jwt:AccessExpirationMinutes"]!),
            TokenType.Refresh => double.Parse(_configuration["Jwt:RefreshExpirationDays"]!) * 24 * 60,
            TokenType.ResetPassword => double.Parse(_configuration["Jwt:ResetPasswordExpirationMinutes"]!),
            TokenType.VerifyEmail => double.Parse(_configuration["Jwt:VerifyEmailExpirationMinutes"]!),
            _ => 30
        };

        var expires = DateTime.UtcNow.AddMinutes(expirationMinutes);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return Task.FromResult((new JwtSecurityTokenHandler().WriteToken(token), expires));
    }

    public int? ValidateToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var secretKey = _configuration["Jwt:Secret"];
        var key = Encoding.UTF8.GetBytes(secretKey!);

        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "sub").Value);

            return userId;
        }
        catch
        {
            return null;
        }
    }
}