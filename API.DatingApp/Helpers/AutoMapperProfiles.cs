using API.DatingApp.Dtos;
using API.DatingApp.Models;
using AutoMapper;

namespace API.DatingApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
     public AutoMapperProfiles()
     {
         CreateMap<User, UserForListDto>();
         CreateMap<User, UserForDetailedDto>();
     }   
    }
}