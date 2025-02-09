using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Extensions;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.MapperProfiles;

public class FormMapperProfile : Profile
{
    public FormMapperProfile()
    {
        // Mapping for FormDetailModel
        CreateMap<FormEntity, FormDetailModel>()
            .ForMember(dest => dest.User, opt => opt.Ignore()) // Ignoring user mapping
            .ForMember(dest => dest.Questions, opt => opt.MapFrom(src => src.Questions)); // Map Questions

        // Mapping for FormListModel
        CreateMap<FormEntity, FormListModel>();

        // Reverse mapping for FormDetailModel to FormEntity
        CreateMap<FormDetailModel, FormEntity>()
            // Ignoring Questions
            .Ignore(entity => entity.OwnerId)
            .MapMember(model => model.Questions, entity => entity.Questions);
    }
}