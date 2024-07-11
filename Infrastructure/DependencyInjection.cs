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

            services.AddSingleton<IEmailSender>(provider =>
            {
                var smtpServer = "smtp.gmail.com";
                var port = 587; 
                var fromEmail = configuration.GetConnectionString("Email"); 
                var password = configuration.GetConnectionString("Password");
            
                return new EmailSender(smtpServer, port, fromEmail, password);
            });

        
            services.AddScoped<GetEmailService>();
            services.AddScoped<IDataContext, ApplicationDbContext>();
            services.AddScoped<IAccountFactory, AccountFactory>();
            services.AddScoped<IPasswordHasher, PasswordHasher>();
            
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
