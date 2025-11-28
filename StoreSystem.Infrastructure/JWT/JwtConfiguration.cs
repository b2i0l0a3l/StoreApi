
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace StoreSystem.Infrastructure.JWT
{
    public  static class JwtConfiguration 
    {
         public static void AddJwtConfiguration(this IServiceCollection services,IConfiguration configuration)
        {
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options =>
                {
                    options.SaveToken = true;
                    options.RequireHttpsMetadata = false;
                    
                    var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? configuration["JWT:secret"];
                    var validAudience = Environment.GetEnvironmentVariable("JWT_VALID_AUDIENCE") ?? configuration["JWT:ValidAudience"];
                    var validIssuer = Environment.GetEnvironmentVariable("JWT_VALID_ISSUER") ?? configuration["JWT:ValidIssuer"];
                    
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidAudience = validAudience,
                        ValidIssuer = validIssuer,
                        ClockSkew = TimeSpan.Zero,
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret!))
                    };
                     options.Events = new JwtBearerEvents
        {
            
        };
                });
        }

    }
}