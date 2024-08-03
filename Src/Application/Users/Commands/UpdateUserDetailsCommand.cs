using Application.Common.ResultsModel;  
using Application.Interfaces;  
using Domain.Entities;  
using MediatR;  
using Microsoft.AspNetCore.Identity;  
using System.Threading;  
using System.Threading.Tasks;  
using Application.Dto;  

namespace Application.Users.Commands  
{  
    public class UpdateUserDetailsCommand : IRequest<Result>  
    {  
        public string Email { get; set; }  
        
        public UpdateDetailsDto UpdateDetailsDto { get; set; } // Property name updated to camel case  
    }  

    public class UpdateUserDetailsCommandHandler : IRequestHandler<UpdateUserDetailsCommand, Result>  
    {  
        private readonly UserManager<ApplicationUser> _userManager;  

        public UpdateUserDetailsCommandHandler(UserManager<ApplicationUser> userManager)  
        {  
            _userManager = userManager;  
        }  

        public async Task<Result> Handle(UpdateUserDetailsCommand request, CancellationToken cancellationToken)  
        {  
            ApplicationUser user = await _userManager.FindByEmailAsync(request.Email);  
            if (user == null)  
            {  
                return Result.Failure<UpdateUserDetailsCommand>("User not found.");  
            }  

            // Ensure that the updateDetailsDto is not null  
            if (request.UpdateDetailsDto == null)  
            {  
                return Result.Failure<UpdateUserDetailsCommand>("Update details are required.");  
            }  

            // Update user details  
            user.UserName = request.UpdateDetailsDto.UserName;  
            user.FirstName = request.UpdateDetailsDto.FirstName;  
            user.LastName = request.UpdateDetailsDto.LastName;  

            IdentityResult updateResult = await _userManager.UpdateAsync(user);  
            if (updateResult.Succeeded)  
            {  
                return Result.Success<UpdateUserDetailsCommand>("User details updated successfully.", user);  
            }  

            return Result.Failure<UpdateUserDetailsCommand>("Failed to update user details.");  
        }  
    }  
}