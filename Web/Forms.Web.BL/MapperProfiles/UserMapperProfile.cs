using AutoMapper;
using Forms.Common.Models.User;

namespace Forms.Web.BL.MapperProfiles;

public class UserMapperProfile : Profile
{
    public UserMapperProfile()
    {
        CreateMap<UserDetailModel, UserListModel>();
    }
}