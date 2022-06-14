using Auth.Dto;
using Auth.Models;
using AutoMapper;
using MongoDB.Driver.GeoJsonObjectModel;

namespace Auth.Mapper;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<SignUpRequest, User>()
        .ForMember(s => s.Password, opt => opt.MapFrom(src => BCrypt.Net.BCrypt.HashPassword(src.Password)));

        CreateMap<PrivateEventLiveStreamInfo, PublicEventLiveStreamInfo>();
        CreateMap<Event, EventByCategoryResponse>();
        CreateMap<User, SignUpResponse>()
        .ForMember(res => res.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<CreateEventRequestAddress, EventAddress>();
        CreateMap<CreateEventRequest, Event>()
        .ForMember( dest => dest.Location, 
                    opt => opt.MapFrom(src => GeoJson.Point(GeoJson.Position(src.Longitute, src.Latitude)))
        );
        
           
        CreateMap<Event, CreatedEventResponse>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Coordinates.Y))
            .ForMember(dest => dest.Longitute, opt => opt.MapFrom(src => src.Location.Coordinates.X));

        CreateMap<NearEventDto, NearEvent>()
        .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Location.Coordinates.Y))
        .ForMember(dest => dest.Longitute, opt => opt.MapFrom(src => src.Location.Coordinates.X))
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<mongoidentity.ApplicationUser, SignInUserResponse>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));

        CreateMap<mongoidentity.ApplicationUser, UserPublishedDto>()
        .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id.ToString()));
    }
}