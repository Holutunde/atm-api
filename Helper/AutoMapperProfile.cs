using AutoMapper;
using ATMAPI.Dto;
using ATMAPI.Models;


namespace ATMAPI.Helper 
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