using System.Reflection;
using StoreSystem.Core.Entities;
using StoreApi.Api.Extension;
using StoreSystem.Application.ServiceRegistration;
using StoreSystem.Infrastructure.InfrastructureRegistration;
using StoreSystem.Infrastructure.JWT;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StoreSystem.Infrastructure.Persistence;
using StoreSystem.Infrastructure.Seed;

// Load environment variables from .env file
DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());


builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

// Add Memory Cache for performance optimization
builder.Services.AddMemoryCache();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:5107","http://127.0.0.1:5500", "http://localhost:5500")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddSwaggerConfig();
builder.Services.AddJwtConfiguration(builder.Configuration);



builder.Services.AddTransient<StoreSystem.Core.Interfaces.IEmailSender, StoreSystem.Infrastructure.Email.SmtpEmailSender>();
builder.Services.AddInfrastructureRegistration();
builder.Services.AddApplicationRegistration();




var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store API V1");
        c.RoutePrefix = "";

        c.OAuthClientId("dev-client");        
        c.OAuthUsePkce();                    
        c.OAuthScopes("openid", "email", "offline_access");
        c.OAuthUseBasicAuthenticationWithAccessCodeGrant();
        c.OAuth2RedirectUrl("http://localhost:5107/api/Auth/ExternalLoginCallback");

    });

}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAll"); 

app.UseAuthentication(); 
app.UseAuthorization();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    await SeedData.InitializeAsync(services);
}

app.Run();