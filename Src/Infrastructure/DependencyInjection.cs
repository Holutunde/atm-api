using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration )
        {

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
        
            services.AddScoped<GetEmailService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IDataContext, ApplicationDbContext>();
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IPasswordHasher, PasswordService>();
            
            services.AddScoped<IJwtTokenService, JwtTokenService>(provider =>
            {
                var key = configuration["Jwt:Key"];
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                return new JwtTokenService(key, issuer, audience);
            });
            

            return services;
        }
    }
}
