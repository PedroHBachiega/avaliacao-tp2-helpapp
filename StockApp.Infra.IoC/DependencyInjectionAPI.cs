using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StockApp.Application.Interfaces;
using StockApp.Application.Mappings;
using StockApp.Application.Services;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Context;
using StockApp.Infra.Data.Identity;
using StockApp.Infra.Data.Repositories;


namespace StockApp.Infra.IoC
{
    public static class DependencyInjectionAPI
    {
        public static IServiceCollection AddInfrastructureAPI(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
             options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"
            ), b => b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName)));

            services.AddScoped<ICategoryRepository, CategoryRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<ISupplierContractRepository, SupplierContractRepository>();
            services.AddScoped<ISupplierEvaluationRepository, SupplierEvaluationRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IProductService, ProductService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<ISupplierManagementService, SupplierManagementService>();
            services.AddScoped<ISupplierRelationshipManagementService, SupplierRelationshipManagementService>();
            services.AddScoped<ISentimentAnalysisService, SentimentAnalysisService>();
            services.AddScoped<IAuthService, AuthService>();

            services.AddAutoMapper(typeof(DomainToDTOMappingProfile));

            var myhandlers = AppDomain.CurrentDomain.Load("StockApp.Application");
            services.AddMediatR(myhandlers);

            // Registrar o serviço de entrega com HttpClient
            services.AddHttpClient<IDeliveryService, DeliveryService>(client =>
            {
                client.BaseAddress = new Uri("https://api.delivery.com/");
            });

            return services;
        }
    }
}
