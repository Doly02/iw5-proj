using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Extensions;
using Forms.Common.Models.Question;
using Forms.Common.Models.Search;

namespace Forms.Api.BL.MapperProfiles
{
    public class QuestionMapperProfile : Profile
    {
        public QuestionMapperProfile()
        {
            CreateMap<QuestionEntity, QuestionDetailModel>()
                .Ignore(dst => dst.Responses);
            
            CreateMap<QuestionEntity, QuestionListModel>();

            CreateMap<QuestionDetailModel, QuestionEntity>()
                .ForMember(dest => dest.Responses, opt => opt.Ignore());
            
            CreateMap<QuestionEntity, SearchResultModel>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(_ => "Question"))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))             
                .MapMember(dest => dest.Description, src => src.Description);
        }
    }
}