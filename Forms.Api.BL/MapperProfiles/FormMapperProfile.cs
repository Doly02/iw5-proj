using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Form;
using Forms.Common.Models.User;

namespace Forms.Api.BL.MapperProfiles;

public class FormMapperProfile : Profile
{
    public FormMapperProfile()
    {
        // Mapping for UserDetailModel
        CreateMap<FormEntity, FormDetailModel>();
        
        // Mapping for UserListModel
        CreateMap<FormEntity, FormListModel>();
        
        // Reverse mapping back to UserEntity
        CreateMap<FormDetailModel, FormEntity>()
            // Ignoring Questions
            .ForMember(dest => dest.Questions, opt => opt.Ignore())
            // Ignoring User
            .ForMember(dest => dest.User, opt => opt.Ignore());
    }
}