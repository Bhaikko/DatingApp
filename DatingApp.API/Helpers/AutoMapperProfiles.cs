using System.Linq;
using AutoMapper;
using DatingApp.API.Dtos;
using DatingApp.API.Models;

namespace DatingApp.API.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // MapFrom only allows redirection of properties
            CreateMap<User, UserForListDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url); 
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom((src, dest) => src.DateOfBirth.CalculateAge());
                });

            CreateMap<User, UserForDetailedDto>()
                .ForMember(dest => dest.PhotoUrl, opt => {
                    opt.MapFrom(src => src.Photos.FirstOrDefault(p => p.IsMain).Url); 
                })
                .ForMember(dest => dest.Age, opt => {
                    opt.MapFrom((src, dest) => src.DateOfBirth.CalculateAge());
                });

            CreateMap<Photo, PhotosForDetailedDto>();

            CreateMap<UserForUpdateDto, User>();

            CreateMap<Photo, PhotoForReturnDto>();

            CreateMap<PhotoForCreationDto, Photo>();

            CreateMap<UserForRegisterDto, User>();

            CreateMap<MessageForCreationDto, Message>().ReverseMap();

            CreateMap<Message, MessageToReturnDto>()
                .ForMember(dest => dest.SenderPhotoUrl, opt => opt.MapFrom(u => u.Sender.Photos.FirstOrDefault(p => p.IsMain).Url))
                .ForMember(dest => dest.RecipientPhotoUrl, opt => opt.MapFrom(u => u.Recipient.Photos.FirstOrDefault(p => p.IsMain).Url));

        }
    }
}