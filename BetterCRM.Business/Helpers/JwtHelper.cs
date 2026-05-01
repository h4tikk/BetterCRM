using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using BetterCRM.Core.Interfaces.Services;

namespace BetterCRM.Business.Helpers
{
    public class JwtHelper
    {
        private readonly string _key;
        private readonly string _issuer;
        private readonly string _audience;
        private readonly int _expiryMinutes;

        public JwtHelper(IConfiguration config)
        {
            _key = config["Jwt:Key"] ?? throw new ArgumentNullException("Jwt:Key");
            _issuer = config["Jwt:Issuer"] ?? "RemoteWorkApi";
            _audience = config["Jwt:Audience"] ?? "RemoteWorkClient";
            _expiryMinutes = int.TryParse(config["Jwt:ExpiryMinutes"], out var m) ? m : 1440;
        }

        public string GenerateToken(CurrentUserInfo user)
        {
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("OrganizationId", user.OrganizationId.ToString()),
            new Claim("DepartmentId", user.DepartmentId?.ToString() ?? ""),
            new Claim("IsMainDirector", user.IsMainDirector.ToString().ToLower()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

            var creds = new SigningCredentials(new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key)), SecurityAlgorithms.HmacSha256);
            var token = new JwtSecurityToken(_issuer, _audience, claims, expires: DateTime.UtcNow.AddMinutes(_expiryMinutes), signingCredentials: creds);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
