﻿using System.Security.Claims;
using AutoMapper;
using Forms.IdentityProvider.BL.Models;

namespace Forms.IdentityProvider.BL.MapperProfiles;

public class AppUserClaimMapperProfile : Profile
{
    public AppUserClaimMapperProfile()
    {
        CreateMap<Claim, AppUserClaimListModel>()
            .ForMember(dest => dest.ClaimType, opt => opt.MapFrom(src => src.Type))
            .ForMember(dest => dest.ClaimValue, opt => opt.MapFrom(src => src.Value));
    }
}
