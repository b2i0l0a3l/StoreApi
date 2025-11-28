using StoreSystem.Core.Interfaces;
using System.Reflection;
using StoreSystem.Application.Contract.Categories.Validator;
using StoreSystem.Application.Interfaces;
using StoreSystem.Application.Contract.ProductContract.Validator;
using StoreSystem.Application.Services.CategoryService;
using StoreSystem.Application.Interfaces.Auth;
using StoreSystem.Application.Services.authService;
using StoreSystem.Application.Services.AuthService.Login;
using StoreSystem.Application.Services.AuthService.Refresh;
using StoreSystem.Application.Services.AuthService.Register;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using StoreSystem.Application.Contract.StockMovementContract.validator;
using StoreSystem.Application.Contract.StoreContract.validator;
using StoreSystem.Application.Services.ProductService;
using StoreSystem.Application.Services.StockMovementService;
using StoreSystem.Application.Services.StoreService;
using StoreSystem.Application.Services.DashboardService;
using StoreSystem.Application.Services.UserService;
using StoreSystem.Application.Contract.Auth.Login.req;
using StoreSystem.Application.Contract.Auth.Res;
using StoreSystem.Application.Services.InventoryService;
using StoreSystem.Application.Services.StockService;
using StoreSystem.Application.Services.SupplierService;
using StoreSystem.Application.Services.CustomerService;
using StoreSystem.Application.Services.PaymentService;

namespace StoreSystem.Application.ServiceRegistration
{
    public static class ApplicationRegistration
    {
        public static void AddApplicationRegistration(this IServiceCollection services)
        {
            services.AddValidatorsFromAssemblyContaining<CategoryValidator>();
            services.AddValidatorsFromAssemblyContaining<StockMovementValidator>();
            services.AddValidatorsFromAssemblyContaining<StoreValidator>();
            services.AddValidatorsFromAssemblyContaining<ProductValidator>();

            services.AddScoped<IUserService, UserService>();
            services.AddScoped<ILogin< AuthRes,GoogleLoginReq>, GoogleLoginService>();
            services.AddScoped<ILogin< AuthRes, LoginModel>, LoginService>();
            services.AddScoped<IRegister, RegisterService>();
            services.AddScoped<IRefresh, RefreshService>();
            services.AddScoped<IAuth, AuthService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IProductService, ProductService>();
            // services.AddScoped<IPurchaseService, PurchaseService>();
            services.AddScoped<IStoreService, StoreService>();
            services.AddScoped<IStockMovementService, StockMovementService>();
            services.AddScoped<IInventoryService, InventoryService>();
            services.AddScoped<IStockService, StockService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IEmployeeService, StoreSystem.Application.Services.EmployeeService.EmployeeService>();
            services.AddScoped<ISupplierProductService, StoreSystem.Application.Services.SupplierProductService.SupplierProductService>();
            services.AddScoped<IDashboardService, DashboardService>();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
        }

    }
}