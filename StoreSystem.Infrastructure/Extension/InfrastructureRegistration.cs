using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StoreSystem.Core.Interfaces;
using StoreSystem.Infrastructure.Persistence.Repo;
using Microsoft.Extensions.DependencyInjection;
using StoreSystem.Infrastructure.EventBus;
using StoreSystem.Infrastructure.Persistence;
using StoreSystem.Application.Interfaces;
using StoreSystem.Infrastructure.External.Auth;
using StoreSystem.Infrastructure.External.Util;

namespace StoreSystem.Infrastructure.InfrastructureRegistration
{
    public static class InfrastructureRegistration 
    {
        public static void AddInfrastructureRegistration(this IServiceCollection services)
        {
            services.AddScoped(typeof(IRepository<>), typeof(Reposatory<>));
            services.AddScoped<IEventBus, MediatRPublisher>();
            services.AddScoped<IUniteOfWork, UniteOfWork>();
            services.AddScoped<IGoogleLoginExtension, GoogleLoginExtension>();
            services.AddScoped<IRoleExtension, UserRoleService>();
            services.AddScoped<IUserClaimsExtension, UserClaimsService>();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            services.AddHttpContextAccessor();
            services.AddScoped<IToken, TokenService>();
            services.AddScoped<IPermissionService, PermissionService>();
        }
    }
}