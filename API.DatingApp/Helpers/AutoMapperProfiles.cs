using System.Linq;
using API.DatingApp.Dtos;
using API.DatingApp.Models;
using AutoMapper;

namespace API.DatingApp.Helpers
{
    public class AutoMapperProfiles : Profile
    {
     public AutoMapperProfiles()
     {
         CreateMap<User, UserForListDto>()
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.MapFrom(d => d.DateOfBirth.CalculateAge());
            });
         CreateMap<User, UserForDetailedDto>() 
            .ForMember(dest => dest.PhotoUrl, opt => {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
            .ForMember(dest => dest.Age, opt => {
                opt.MapFrom(d => d.DateOfBirth.CalculateAge());
            });

        //Definicja Mapowania DTO 
        //CreateMap<Source, Destiny>
         CreateMap<Photo, PhotosForDetailedDto>();
         CreateMap<UserForUpdateDto, User>();
         CreateMap<Photo, PhotoForReturnDto>();
         CreateMap<PhotoForCreationDto, Photo>();
         CreateMap<UserForRegisterDto, User>();
         //Mapowanie MessageForCreationDto=>Message oraz Message=>MessageForCreationDto
         //Używajac ReverseMap
         CreateMap<MessageForCreationDto, Message>().ReverseMap();
     }   
    }
}