
using Forms.Common.BL.Facades;
using Forms.IdentityProvider.BL.Models;

namespace Forms.IdentityProvider.BL.Facades;

public interface IAppUserClaimsFacade : IAppFacade
{
    Task<IEnumerable<AppUserClaimListModel>> GetAppUserClaimsByUserIdAsync(Guid userId);
}
