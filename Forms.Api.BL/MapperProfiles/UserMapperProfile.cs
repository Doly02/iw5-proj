using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.User;
using Forms.Common.Extensions;

namespace Forms.Api.BL.MapperProfiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        // Mapování pro UserDetailModel
        CreateMap<UserEntity, UserDetailModel>();

        // Mapování pro UserListModel
        CreateMap<UserEntity, UserListModel>();
        // Reverzní mapování zpět do UserEntity
        CreateMap<UserDetailModel, UserEntity>()
            .ForMember(dest => dest.Responses, opt => opt.Ignore()) // Ignorujeme Responses
            .ForMember(dest => dest.Forms, opt => opt.Ignore()); // Ignorujeme Forms

        CreateMap<UserListModel, UserEntity>()
            .ForMember(dest => dest.PasswordHash, opt => opt.Ignore()) // Ignorujeme PasswordHash
            .ForMember(dest => dest.Responses, opt => opt.Ignore()) // Ignorujeme Responses
            .ForMember(dest => dest.Forms, opt => opt.Ignore()); // Ignorujeme Forms
    }
}
