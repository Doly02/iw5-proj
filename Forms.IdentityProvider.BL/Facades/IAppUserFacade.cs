﻿using Forms.Common.BL.Facades;
using Forms.Common.Models.User;
using Forms.IdentityProvider.BL.Models;

namespace Forms.IdentityProvider.BL.Facades;

public interface IAppUserFacade : IAppFacade
{
    Task<Guid?> CreateAppUserAsync(AppUserCreateModel appUserModel);
    Task<bool> ValidateCredentialsAsync(string userName, string password);
    Task<Guid> GetUserIdByUserNameAsync(string userName);
    public Task<AppUserDetailModel?> GetUserByIdAsync(Guid id);

    Task<IList<AppUserListModel>> SearchAsync(string searchString);
    Task<AppUserDetailModel?> GetUserByUserNameAsync(string userName);
    Task<AppUserDetailModel?> GetAppUserByExternalProviderAsync(string provider, string providerIdentityKey);
    Task<AppUserDetailModel> CreateExternalAppUserAsync(AppUserExternalCreateModel appUserModel);
    Task<bool> ActivateUserAsync(string securityCode, string email);
    Task<bool> IsEmailConfirmedAsync(string userName);
}
