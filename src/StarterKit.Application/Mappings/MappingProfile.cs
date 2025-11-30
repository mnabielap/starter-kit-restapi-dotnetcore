using AutoMapper;
using StarterKit.Application.DTOs.Users;
using StarterKit.Domain.Entities;
using StarterKit.Domain.Enums;

namespace StarterKit.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        // Entity -> DTO
        CreateMap<User, UserResponse>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role.ToString().ToLower()))
            .ForMember(dest => dest.IsEmailVerified, opt => opt.MapFrom(src => src.IsEmailVerified ? 1 : 0));

        // DTO -> Entity
        CreateMap<CreateUserRequest, User>()
            .ForMember(dest => dest.Role, opt => opt.MapFrom(src => Enum.Parse<Role>(src.Role, true)));

        CreateMap<UpdateUserRequest, User>()
             .ForMember(dest => dest.Role, opt => opt.Ignore()) // Handled manually or conditionally
             .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));
    }
}