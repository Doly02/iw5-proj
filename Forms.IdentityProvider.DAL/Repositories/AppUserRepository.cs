using Forms.IdentityProvider.DAL;
using Forms.IdentityProvider.DAL.Entities;
using Forms.IdentityProvider.DAL.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forms.IdentityProvider.DAL.Repositories;

public class AppUserRepository(IdentityProviderDbContext identityProviderDbContext) : IAppUserRepository
{
    public async Task<IList<AppUserEntity>> SearchAsync(string searchString)
        => await identityProviderDbContext.Users.Where(user => EF.Functions.Like(user.UserName, $"%{searchString}%"))
        .ToListAsync();
}
