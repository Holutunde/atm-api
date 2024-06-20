using AutoMapper;
using Application.Dto;
using Domain.Entities;


namespace Api.Helper 
{
    public class AutoMapperProfile : Profile
  {
    public AutoMapperProfile()
    {
        CreateMap<User, UserUpdateDto>();
        CreateMap<UserUpdateDto,User>();
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();
        CreateMap<Admin, AdminDto>();
        CreateMap<AdminDto, Admin>();
        
    }
  }

}