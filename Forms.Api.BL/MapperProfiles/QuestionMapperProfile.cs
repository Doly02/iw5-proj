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
        CreateMap<QuestionEntity, QuestionDetailModel>()
            .Ignore(dest => dest.Responses); 

        CreateMap<QuestionEntity, QuestionListModel>();

        CreateMap<QuestionDetailModel, QuestionEntity>()
            .Ignore(dest => dest.Responses)  
            .Ignore(dest => dest.Form)       
            .Ignore(dest => dest.FormId);    

        CreateMap<QuestionListModel, QuestionEntity>()
            .Ignore(dest => dest.Form)       
            .Ignore(dest => dest.Responses)  
            .Ignore(dest => dest.FormId);  
        
        CreateMap<QuestionEntity, SearchResultModel>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "Question"))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))             
            .MapMember(dest => dest.Description, src => src.Description);
    }
}
