using AutoMapper;
using Forms.Web.BL.Options;
using Forms.Web.DAL.Repositories;
using Forms.Common.Models.User;
using Microsoft.Extensions.Options;

namespace Forms.Web.BL.Facades;

public class UserFacade : FacadeBase<UserDetailModel, UserListModel>
{
    private readonly IUserApiClient authorizedApiClient;
    private readonly IUserApiClient anonymousApiClient;
    
    public UserFacade(
        Func<string, IUserApiClient> apiClientFactory,
        UserRepository userRepository,
        IMapper mapper,
        IOptions<LocalDbOptions> localDbOptions)
        : base(userRepository, mapper, localDbOptions)
    {
        authorizedApiClient = apiClientFactory("api");
        anonymousApiClient = apiClientFactory("AnonymousApi");
    }
    
    public override async Task<List<UserListModel>> GetAllAsync()
    {
        var usersAll = await base.GetAllAsync();

        var usersFromApi = await anonymousApiClient.UserGetAsync(culture);
        foreach (var userFromApi in usersFromApi)
        {
            if (usersAll.Any(r => r.Id == userFromApi.Id) is false)
            {
                usersAll.Add(userFromApi);
            }
        }

        return usersAll;
    }

    public override async Task<UserDetailModel> GetByIdAsync(Guid id)
    {
        return await anonymousApiClient.UserGetAsync(id, culture);
    }

    protected override async Task<Guid> SaveToApiAsync(UserDetailModel data)
    {
        return await anonymousApiClient.UpsertAsync(culture, data);
    }

    public override async Task DeleteAsync(Guid id)
    {
        await authorizedApiClient.UserDeleteAsync(id, culture);
    }
}