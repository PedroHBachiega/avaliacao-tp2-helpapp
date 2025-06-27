using System.Globalization;
using StockApp.Application.Interfaces;
using StockApp.Infra.IoC;
using StockApp.Infrastructure.Services;
using StockApp.API.Infrastructure.Middlewares;
using Microsoft.Extensions.Logging;
using StockApp.API.Infrastructure.Filters;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();
        builder.Logging.AddDebug();
        
        // Add services to the container.
        builder.Services.AddInfrastructureAPI(builder.Configuration);

        builder.Services.AddScoped<IUserService, UserService>();    
        
        builder.Services.AddControllers(options =>
        {
            options.Filters.Add<GlobalExceptionFilter>();
        });

        // Cors

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("CorsPolicy", policy =>
            {
                if (builder.Environment.IsDevelopment())
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
                else
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                }
            });
        });


    builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        
        app.UseCors("CorsPolicy");

        app.UseMiddleware<ErrorHandlingMiddleware>();
        
        app.UseRouting();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}