using Forms.IdentityProvider.DAL.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Forms.IdentityProvider.DAL;

public class IdentityProviderDbContext : IdentityDbContext<AppUserEntity, AppRoleEntity, Guid, AppUserClaimEntity, AppUserRoleEntity, AppUserLoginEntity, AppRoleClaimEntity, AppUserTokenEntity>
{
    public IdentityProviderDbContext(DbContextOptions options)
        : base(options)
    {
    }
}
