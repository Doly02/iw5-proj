using Forms.IdentityProvider.DAL.Entities;

namespace Forms.IdentityProvider.DAL.Repositories;

public interface IAppUserRepository
{
    Task<IList<AppUserEntity>> SearchAsync(string searchString);
}
