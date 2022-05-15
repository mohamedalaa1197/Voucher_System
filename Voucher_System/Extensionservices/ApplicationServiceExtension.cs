using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Voucher_System.Models;
using Voucher_System.Services.Implementation;
using Voucher_System.Services.Interfaces;

namespace API.Extensionservices
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection applicationServices(this IServiceCollection services, IConfiguration config)
        {
            services.AddDbContext<ApplicationContext>(options =>
            {
                options.UseSqlServer(config.GetConnectionString("VoucherDB"));
            });

            services.AddScoped<ITokenService, TokenService>();
            services.AddCors();

            return services;

        }
    }
}