using StockApp.Application.Interfaces;
using StockApp.Infra.IoC;
using StockApp.Infra.Data.Services;
using StockApp.Infrastructure.Services;
using StockApp.API.Infrastructure.Middlewares;
using StockApp.API.Infrastructure.Filters;
using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using StockApp.Domain.Interfaces;
using StockApp.Infra.Data.Repositories;
using Serilog;
using Serilog.Events;
using StockApp.Application.Services;
using StockApp.API.Infrastructure.Authorization;
using Microsoft.AspNetCore.Authorization;
using DeliveryServiceImpl = StockApp.Infra.Data.Services.DeliveryService;
using StockApp.Application.Interfaces;
using System.Runtime.ConstrainedExecution;

public class Program
{
    private static void Main(string[] args)
    {
        // Configurar Serilog a partir do appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production"}.json", optional: true)
            .Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        try
        {
            Log.Information("Iniciando aplicação StockApp");

            var builder = WebApplication.CreateBuilder(args);

            // Configurar Serilog como provider de logging
            builder.Host.UseSerilog();

            builder.Services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = builder.Configuration.GetConnectionString("Redis");
                options.InstanceName = "StockApp";
            });

            builder.Services.AddScoped<IAuditService, AuditService>();
            builder.Services.AddInfrastructureAPI(builder.Configuration);
            builder.Services.AddScoped<IUserService, StockApp.Infrastructure.Services.UserService>();
            builder.Services.AddScoped<IProductRepository, StockApp.Infra.Data.Repositories.ProductRepository>();
            builder.Services.AddScoped<IInventoryService, StockApp.Application.Services.InventoryService>();
            builder.Services.AddScoped<ICacheService, StockApp.Infra.Data.Services.CacheService>();
            builder.Services.AddScoped<IMfaService, StockApp.Infra.Data.Services.MfaService>();
            builder.Services.AddScoped<IJustInTimeInventoryService, StockApp.Infra.Data.Services.JustInTimeInventoryService>();
            builder.Services.AddScoped<IEmployeePerformanceEvaluationService, StockApp.Application.Services.EmployeePerformanceEvaluationService>();
            builder.Services.AddScoped<ISalesPredictionService, StockApp.Application.Services.SalesPredictionService>();
            builder.Services.AddScoped<ICustomReportService, CustomReportService>();

            builder.Services.AddControllers(options =>
            {
                options.Filters.Add<GlobalExceptionFilter>();
            });

            // Cors

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });


            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "StockApp API",
                    Version = "v1",
                    Description = "API para gerenciamento de estoque",
                    Contact = new OpenApiContact
                    {
                        Name = "StockApp Team",
                        Email = "support@stockapp.com"
                    }
                });


                var xmlFile = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);


                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };
                
                c.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                    {
                        securitySchema,
                        new string[] {}
                    }
                };
                
                c.AddSecurityRequirement(securityRequirement);
            });

            var jwtSettings = builder.Configuration.GetSection("Jwt");

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings["Issuer"],
                    ValidAudience = jwtSettings["Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
                };
            })
            .AddGoogle(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
                options.SaveTokens = true;
            })
            .AddFacebook(options =>
            {
                options.AppId = builder.Configuration["Authentication:Facebook:AppId"];
                options.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"];
                options.SaveTokens = true;
            })
            .AddMicrosoftAccount(options =>
            {
                options.ClientId = builder.Configuration["Authentication:Microsoft:ClientId"];
                options.ClientSecret = builder.Configuration["Authentication:Microsoft:ClientSecret"];
                options.SaveTokens = true;
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));

                options.AddPolicy("CanManageStock", policy =>
                     policy.Requirements.Add(new PermissionRequirement("CanManageStock")));
            });

            builder.Services.AddSingleton<IAuthorizationHandler, PermissionHandler>();

            // Configuração do Rate Limiting
            builder.Services.AddRateLimiting(options =>
            {
                // Política padrão: 100 requisições por minuto
                options.DefaultPolicy = new RateLimitPolicy
                {
                    MaxRequests = 100,
                    Window = TimeSpan.FromMinutes(1)
                };

                // Operações de leitura: 200 requisições por minuto
                options.ReadOperationsPolicy = new RateLimitPolicy
                {
                    MaxRequests = 200,
                    Window = TimeSpan.FromMinutes(1)
                };

                // Operações de escrita: 50 requisições por minuto
                options.WriteOperationsPolicy = new RateLimitPolicy
                {
                    MaxRequests = 50,
                    Window = TimeSpan.FromMinutes(1)
                };

                // Endpoints de autenticação: 10 requisições por 5 minutos
                options.AuthEndpointsPolicy = new RateLimitPolicy
                {
                    MaxRequests = 10,
                    Window = TimeSpan.FromMinutes(5)
                };
            });

            //Registro do DeliveryService para ser injetado via HttpClient//

            builder.Services.AddHttpClient<IDeliveryService, DeliveryServiceImpl>()
                .ConfigureHttpClient(client =>
            {
                client.BaseAddress = new Uri("https://api.delivery.com/");
            });

            var app = builder.Build();


            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            
            // Configurar arquivos estáticos
            app.UseStaticFiles();

            app.UseCors("CorsPolicy");

            // Rate Limiting deve vir antes da autenticação
            app.UseRateLimiting();

            app.UseMiddleware<ErrorHandlingMiddleware>();

            app.UseRouting();

            app.UseAuthentication();

            app.UseAuthorization();


            app.UseMiddleware<RequestLoggingMiddleware>();

            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
                options.GetLevel = (httpContext, elapsed, ex) => ex != null
                    ? LogEventLevel.Error
                    : httpContext.Response.StatusCode > 499
                        ? LogEventLevel.Error
                        : LogEventLevel.Information;
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].FirstOrDefault());
                    diagnosticContext.Set("RemoteIP", httpContext.Connection.RemoteIpAddress?.ToString());
                };
            });

            app.MapControllers();

            Log.Information("Aplicação StockApp iniciada com sucesso");
            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Aplicação falhou ao iniciar");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}