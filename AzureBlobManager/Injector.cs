using AzureBlobManager.Infrastructure.Helpers;
using AzureBlobManager.Infrastructure.Persistence.DataContext;
using AzureBlobManager.Infrastructure.Services.AzureStorage;
using AzureBlobManager.Infrastructure.Services.Common;
using AzureBlobManager.Infrastructure.Services.MediaTypes.DocumentsConfig;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace AzureBlobManager
{
    public static class Injector
    {
        public static IServiceCollection AddAzureBlobConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDataProtection();
            // for cutom authontication
            services.AddHttpContextAccessor();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthorization();

            services.AddAutoMapper(Assembly.GetExecutingAssembly());                     
            services.AddDbContext<DocumentDbContext>(options =>
                options.UseSqlServer("Data Source=.;Initial Catalog=AuthFirstProjectDb;Trusted_Connection=True;Encrypt=False",
                b => {
                    b.MigrationsAssembly(typeof(DocumentDbContext).Assembly.FullName);                   
                }
                ));
            services.AddDbContext<DocumentDbContext>(options =>
              options.UseSqlServer("Data Source=.;Initial Catalog=MicrosoftDb;Trusted_Connection=True;Encrypt=False",
              b => {
                  b.MigrationsAssembly(typeof(DocumentDbContext).Assembly.FullName);
              }
              ));
            services.AddDbContext<DocumentDbContext>(options =>
              options.UseSqlServer("Data Source=.;Initial Catalog=linkedInDb;Trusted_Connection=True;Encrypt=False",
              b => {
                  b.MigrationsAssembly(typeof(DocumentDbContext).Assembly.FullName);
              }
              ));
            services.AddDbContext<DocumentDbContext>(options =>
              options.UseSqlServer("Data Source=.;Initial Catalog=twitterDb;Trusted_Connection=True;Encrypt=False",
              b => {
                  b.MigrationsAssembly(typeof(DocumentDbContext).Assembly.FullName);
              }
              ));
            services.AddScoped<IDocumentDbContext>(provider => provider.GetService<DocumentDbContext>()!);

            services.AddTransient<ICommonService, CommonService>();
            services.AddTransient<IDocumentsConfigService, DocumentsConfigService>();
            services.AddTransient<IAzureStorageService, AzureStorageService>();
            services.AddTransient<IConfigHelper, ConfigHelper>();
            return services;
        }
    }
}
