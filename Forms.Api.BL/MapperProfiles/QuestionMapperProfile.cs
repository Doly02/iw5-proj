using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Extensions;
using Forms.Common.Models.Question;

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
        }
    }
}