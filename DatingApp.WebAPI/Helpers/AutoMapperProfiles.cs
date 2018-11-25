using System;
using System.Linq;
using AutoMapper;
using DatingApp.WebAPI.DTOs;
using DatingApp.WebAPI.Models;

namespace DatingApp.WebAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UsersListDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
                  {
                      opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
                  })
                .ForMember(dest => dest.Age, opt =>
              {
                  opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
              });

            CreateMap<User, UserDto>()
                .ForMember(dest => dest.PhotoUrl, opt =>
            {
                opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url);
            })
                .ForMember(dest => dest.Age, opt =>
            {
                opt.ResolveUsing(src => src.DateOfBirth.CalculateAge());
            }); ;
            CreateMap<Photo, PhotoForUserDto>();
            CreateMap<userToUpdateDTO, User>();
            CreateMap<PhotoForCreationDto, Photo>();
            CreateMap<Photo,PhotoForGetDto>();
        }
    }
}
