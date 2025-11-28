using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using StoreSystem.Core.Interfaces;
using StoreSystem.Core.Entities;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Configuration;
using BookingSystem.Core.common;

namespace StoreSystem.Infrastructure.External.Auth
{
    public class TokenService : IToken
    {
        private readonly IRepository<TokenInfo> _tokenRepository;
        private readonly IConfiguration _configuration;
        public TokenService(Microsoft.Extensions.Configuration.IConfiguration configuration,  IRepository<TokenInfo> tokenRepository)
        {
            _configuration = configuration;
            _tokenRepository = tokenRepository;

        }

        public async Task<(bool,string)> AddTokenToDB(ApplicationUser user, string refreshToken)
        {
            try
            {
                var tokenInfo = await _tokenRepository.FindAsync(x => x.Username == user.UserName!);
                    
                    if (tokenInfo == null)
                    {
                        TokenInfo tokenInfo1 = new()
                        {
                            Username = user.Email!,
                            RefreshToken = refreshToken,
                            Expiration = DateTime.Now.AddDays(7)
                        };
                        await _tokenRepository.AddAsync(tokenInfo1);
                    }
                    else
                    {
                        tokenInfo.RefreshToken = refreshToken;
                        tokenInfo.Expiration = DateTime.Now.AddDays(7);
                        await _tokenRepository.UpdateAsync(tokenInfo);
                    }
                await _tokenRepository.SaveAsync();
                return (true, "Token Add To Db Successfully");
            }catch(Exception ex)
            {
                return (false,ex.Message);
            }
        }

        public string? GenerateTemporaryToken(string UserId,int Minute)
        {
            if (string.IsNullOrEmpty(UserId)) return null;

            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["JWT:Secret"];
            var authSigningKey = new SymmetricSecurityKey
                       (Encoding.UTF8.GetBytes(jwtSecret!));

            List<Claim> claims = new()
            {
                new Claim(ClaimTypes.NameIdentifier, UserId),
                new Claim("Temporary", "True"),
                new Claim(ClaimTypes.Role , Roles.User)
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER") ?? _configuration["JWT:ValidIssuer"],
                Audience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Minute),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }


        public string GenerateAccessToken(IEnumerable<Claim> claims, bool isTemporary = false)
        {
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["JWT:Secret"];
            var authSigningKey = new SymmetricSecurityKey
                       (Encoding.UTF8.GetBytes(jwtSecret!));

            // Add IsTemporary claim
            var claimsList = claims.ToList();
            claimsList.Add(new Claim("IsTemporary", isTemporary.ToString()));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Issuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER") ?? _configuration["JWT:ValidIssuer"],
                Audience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? _configuration["JWT:ValidAudience"],
                Subject = new ClaimsIdentity(claimsList),
                Expires = isTemporary ? DateTime.UtcNow.AddMinutes(30) : DateTime.UtcNow.AddDays(15),
                SigningCredentials = new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
        public string GenerateRefreshToken()
        {
            
            var randomNumber = new byte[32];
            using (var rng = System.Security.Cryptography.RandomNumberGenerator.Create())
            {
                rng.GetBytes(randomNumber);
            }
            return Convert.ToBase64String(randomNumber);

        }
        public ClaimsPrincipal GetPrincipalFromExpiredToken(string accessToken)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? _configuration["JWT:Secret"];
            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER") ?? _configuration["JWT:ValidIssuer"],
                ValidAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? _configuration["JWT:ValidAudience"],
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
                ValidateLifetime = false
            };
            var tokenHandler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
            var principal = tokenHandler.ValidateToken(accessToken, tokenValidationParameters, out SecurityToken securityToken);
            var jwtSecurityToken = securityToken as System.IdentityModel.Tokens.Jwt.JwtSecurityToken;
            if (jwtSecurityToken == null || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                throw new SecurityTokenException("Invalid token");
            return principal;
        }
    }
}