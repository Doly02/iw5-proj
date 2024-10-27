using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Common.Models.Response;
using Forms.Common.Extensions;
using Forms.Common.Models.Response;

namespace Forms.Api.BL.MapperProfiles;

public class ResponseMapperProfile : Profile
{
    public ResponseMapperProfile()
    {
        CreateMap<ResponseEntity, ResponseDetailModel>();

        CreateMap<ResponseEntity, ResponseListModel>();
        
        /* Reverse Mapping */
        CreateMap<ResponseDetailModel, ResponseEntity>()
            .ForMember(dst => dst.User, opt => opt.Ignore())
            .ForMember(dst => dst.Question, opt => opt.Ignore());

        CreateMap<ResponseListModel, ResponseEntity>()
            .ForMember(dst => dst.User, opt => opt.Ignore())
            .ForMember(dst => dst.Question, opt => opt.Ignore());
    }
}