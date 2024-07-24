using Application.Admins.Commands;
using Application.Validator;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Reflection;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());

            services.AddTransient<IValidator<RegisterAdminCommand>, AdminValidator>();
            services.AddTransient<IValidator<RegisterUserCommand>, UserValidator>();
            
            services.AddLogging();
            
            // services.AddSingleton<ILoggerFactory, LoggerFactory>();
            // services.AddSingleton(typeof(ILogger<>), typeof(Logger<>));
            
            return services;
        }
    }
}