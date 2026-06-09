using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using TodoApp.Api.Core.Entities;
using TodoApp.Api.Core.Interfaces;

namespace TodoApp.Api.Infrastructure.Auth
{
    public class JwtProvider : IJwtProvider
    {
        private readonly IConfiguration _configuration;

        public JwtProvider(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string Generate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name)
            };

            var secret = _configuration["Jwt:Secret"];
            if (string.IsNullOrEmpty(secret))
            {
                throw new InvalidOperationException("JWT Secret is not configured.");
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
