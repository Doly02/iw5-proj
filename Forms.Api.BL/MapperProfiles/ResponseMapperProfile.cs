using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Extensions;
using Forms.Common.Models.Response;

namespace Forms.Api.BL.MapperProfiles
{
    public class ResponseMapperProfile : Profile
    {
        public ResponseMapperProfile()
        {
            CreateMap<ResponseEntity, ResponseDetailModel>()
                .Ignore(dst => dst.Question);
            
            CreateMap<ResponseEntity, ResponseListModel>();

            CreateMap<ResponseDetailModel, ResponseEntity>()
                .ForMember(dest => dest.User, opt => opt.Ignore())
                .ForMember(dest => dest.Question, opt => opt.Ignore());
        }
    }
}