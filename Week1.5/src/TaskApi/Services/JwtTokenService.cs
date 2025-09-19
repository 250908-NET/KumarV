using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace TaskApi.Services;

public class JwtTokenService : ITokenService
{
    private readonly IConfiguration _cfg;

    public JwtTokenService(IConfiguration cfg) => _cfg = cfg;

    public string CreateToken(string userId, string userName)
    {
        // read secrets/config from appsettings.json
        var keyString = _cfg["Jwt:Key"] ?? throw new Exception("Missing Jwt:Key in configuration.");
        var issuer = _cfg["Jwt:Issuer"];
        var audience = _cfg["Jwt:Audience"];

        // build signing credentials with the symmetric key
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        // create claims wanted in the token (user id and name)
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId),
            new Claim(ClaimTypes.Name, userName),
        };

        // construct the token with issuer/audience/claims/expiry/signature
        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow.AddHours(8),
            signingCredentials: creds
        );

        // serialize to compact JWT string: "header.payload.signature"
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
