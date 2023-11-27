using Auth.Domain.Entities;
using Auth.Infrastructure.Behaviors;
using Auth.Infrastructure.Models;
using Auth.Infrastructure.Models.MultiTenancy;
using Auth.Infrastructure.Seeds;
using Auth.Infrastructure.Services.Auth;
using Auth.Infrastructure.Services.MultiTenancy;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using System.Text;

namespace Auth.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());  
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(RequestValidationBehavior<,>));
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"),
                b =>
                {
                    b.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                }
                ));
            services.Configure<JwtSetting>(configuration.GetSection("JwtSetting"));
            services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();
            services.AddTransient<IAuthService, AuthService>(); 
            return services;
        }

        public static IServiceCollection AddCorsPolicy(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddCors(options =>
            {
                options.AddPolicy(configuration["CorsPolicy:DefaultPolicy"], policy =>
                {
                    policy.AllowAnyOrigin()
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            return services;
        }

        public static IServiceCollection AddIdentityServer(this IServiceCollection services, IConfiguration configuration)
        {
            // Adding Authentication  
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })

            // Adding Jwt Bearer  
            .AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidIssuer = configuration["JwtSetting:Issuer"],
                    ValidAudience = configuration["JwtSetting:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSetting:Key"] ?? ""))
                };
            });

            return services;
        }

        public static async Task<IServiceCollection> AddTenancyAsync(this IServiceCollection services,
                  ConfigurationManager configuration)
        {
            services.AddScoped<ITenantServices, TenantServices>();
            services.Configure<TenantSetting>(configuration.GetSection(nameof(TenantSetting)));

            TenantSetting options = new();
            configuration.GetSection(nameof(TenantSetting)).Bind(options);

            var defaultDbProvider = options.Defaults.DBProvider;

            if (defaultDbProvider.ToLower() == "mssql")
            {
                services.AddDbContext<ApplicationDbContext>(m => m.UseSqlServer());
            }

            foreach (var tenant in options.Tenant)
            {
                var connectionString = tenant.ConnectionString?? options.Defaults.ConnectionString;

                using var scope = services.BuildServiceProvider().CreateScope();
                var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                dbContext.Database.SetConnectionString(connectionString);

                if (tenant.ConnectionString!=null&& dbContext.Database.GetPendingMigrations().Any())
                {
                    dbContext.Database.Migrate();
                    if((await dbContext.Database.GetAppliedMigrationsAsync()).Count()>1)
                    {
                        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
                        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                        var applicationDbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                        await ContextSeed.Seed(userManager, roleManager, tenant.Id);
                    }
                   
                }
              
            } 

            return services;
        }
 
    }
}
