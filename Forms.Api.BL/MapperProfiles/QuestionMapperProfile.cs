using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Question;
using Forms.Common.Extensions;

namespace Forms.Api.BL.MapperProfiles;

public class QuestionMapperProfile : Profile
{
    public QuestionMapperProfile()
    {
        CreateMap<QuestionEntity, QuestionDetailModel>();

        CreateMap<QuestionEntity, QuestionListModel>();
        
        /* Reverse Mapping */
        CreateMap<QuestionDetailModel, QuestionEntity>()
            .ForMember(dest => dest.Responses, opt => opt.Ignore());

        CreateMap<QuestionListModel, QuestionEntity>()
            .ForMember(dest => dest.Responses, opt => opt.Ignore());
    }
}