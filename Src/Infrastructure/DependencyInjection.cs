using Application.Interfaces;
using Infrastructure.Data;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Mail;
using Infrastructure.Helpers;
using Microsoft.AspNetCore.Identity;
using FluentValidation;
using Application.Users.Commands;


namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            

            services.AddSingleton<SmtpClient>();

            services.AddScoped<IGetEmailService, GetEmailService>();
            services.AddScoped<IEmailSender, EmailService>();
            services.AddScoped<IDataContext, ApplicationDbContext>();
           

            services.AddScoped<ITokenService, TokenService>(provider =>
            {
                var key = configuration["Jwt:Key"];
                var issuer = configuration["Jwt:Issuer"];
                var audience = configuration["Jwt:Audience"];
                return new TokenService(key, issuer, audience);
            });
            

            return services;
        }
    }
}