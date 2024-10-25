using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Question;
using Forms.Common.Extensions;
using Forms.Common.Models.Search;

namespace Forms.Api.BL.MapperProfiles;

public class QuestionMapperProfile : Profile
{
    public QuestionMapperProfile()
    {
        // Mapování z QuestionEntity na QuestionDetailModel
        CreateMap<QuestionEntity, QuestionDetailModel>()
            .Ignore(dest => dest.Responses);  // Ignorujeme Responses, pokud nejsou potřeba

        // Mapování z QuestionEntity na QuestionListModel
        CreateMap<QuestionEntity, QuestionListModel>();

        // Reverzní mapování z QuestionDetailModel na QuestionEntity
        CreateMap<QuestionDetailModel, QuestionEntity>()
            .Ignore(dest => dest.Responses)  // Ignorujeme Responses
            .Ignore(dest => dest.Form)       // Ignorujeme Form
            .Ignore(dest => dest.FormId);    // Ignorujeme FormId, pokud není použit

        // Reverzní mapování z QuestionListModel na QuestionEntity
        CreateMap<QuestionListModel, QuestionEntity>()
            .Ignore(dest => dest.Form)       // Ignorujeme Form
            .Ignore(dest => dest.Responses)       // Ignorujeme Form
            .Ignore(dest => dest.FormId);    // Ignorujeme FormId
    }
}
