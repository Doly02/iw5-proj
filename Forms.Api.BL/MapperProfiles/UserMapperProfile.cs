using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.User;
using Forms.Common.Extensions;
using Forms.Common.Models.Search;

namespace Forms.Api.BL.MapperProfiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserEntity, UserDetailModel>();

        CreateMap<UserEntity, UserListModel>();

        CreateMap<UserDetailModel, UserEntity>()
            .Ignore(entity => entity.OwnerId)
            .ForMember(dest => dest.Responses, opt => opt.Ignore()) // Ignorujeme Responses
            .ForMember(dest => dest.Forms, opt => opt.Ignore()); // Ignorujeme Forms

        // CreateMap<UserListModel, UserEntity>()
        //     .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Ignorujeme PasswordHash
        //     .ForMember(dest => dest.Responses, opt => opt.Ignore()) // Ignorujeme Responses
        //     .ForMember(dest => dest.Forms, opt => opt.Ignore()); // Ignorujeme Forms
        //
        CreateMap<UserEntity, SearchResultModel>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "User"))
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id));
    }
}
