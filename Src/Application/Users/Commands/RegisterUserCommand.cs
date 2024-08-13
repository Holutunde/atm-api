using Application.Common.ResultsModel;
using Application.Extensions;
using Domain.Entities;  
using FluentValidation;  
using FluentValidation.Results; 
using Application.Interfaces;
using Application.User;
using Domain.Enum;
using MediatR;
using Microsoft.AspNetCore.Identity;  

namespace Application.Users.Commands  
{  
    public class RegisterUserCommand : IRequest<Result>  
    {  
        public string Email { get; set; }  
        public string Password { get; set; }  
        public string FirstName { get; set; }  
        public string LastName { get; set; }  
        public int Pin { get; set; }  
        public string Role { get; set; }  
    }  

    public class RegisterUserCommandHandler : IRequestHandler<RegisterUserCommand, Result>  
    {  
        private readonly IEmailSender _emailSender;  
        private readonly UserManager<ApplicationUser> _userManager;  

        public RegisterUserCommandHandler( IEmailSender emailSender, UserManager<ApplicationUser> userManager)  
        {  
            _emailSender = emailSender;  
            _userManager = userManager;  
        }  

        public async Task<Result> Handle(RegisterUserCommand request, CancellationToken cancellationToken)  
        {  
            // Validate the request
            await request.ValidateAsync(new UserValidator(), cancellationToken);
          
            // Check if the email is already registered  
            ApplicationUser? existingApplicationUser = await _userManager.FindByEmailAsync(request.Email);  
            if (existingApplicationUser != null)  
            {  
                return Result.Failure<RegisterUserCommand>("Email is already registered.");  
            }  
            

            // Create a new user object  
            ApplicationUser newApplicationUser = new ApplicationUser  
            {  
                UserName = request.FirstName,  
                Email = request.Email,  
                FirstName = request.FirstName,  
                LastName = request.LastName,  
                PinHash = UserPinHasher.HashPin(request.Pin),  
                Role = Enum.Parse<Roles>(request.Role, true), 
                UserStatus = Status.Active,
                UserStatusDes = Status.Active.ToString(),
                RoleDesc = request.Role,  
                OpeningDate = DateTime.Now,  
                AccountNumber = UserGenerateAccountNumber.GenerateAccountNumber(),  
                GuId = Guid.NewGuid()  
            };  
            
                // Create the user  
                IdentityResult createdUser = await _userManager.CreateAsync(newApplicationUser, request.Password);  

                if (createdUser.Succeeded)  
                {  
                    await _emailSender.SendEmailAsync(request.Email, "Registration Successful", "Welcome to our service!");  
                    return Result.Success<RegisterUserCommand>("User registered successfully.", newApplicationUser );  
                }  
                
                return Result.Failure<RegisterUserCommand>("Error occurred while creating the user: ");  
            
        }  
    }  
}