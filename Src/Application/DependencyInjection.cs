using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Users;
using Application.Users.Commands;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddMediatR(Assembly.GetExecutingAssembly());
            
            
            services.AddLogging();
            
            return services;
        }
    }
}