using Forms.Common.BL.Facades;
using Forms.Common.Models.User;

namespace Forms.Api.BL.Facades;

public interface IUserFacade : IAppFacade, ISearchFacade
{
    List<UserListModel> GetAll();
    UserDetailModel? GetById(Guid id);
    Guid CreateOrUpdate(UserDetailModel userModel, string? ownerId);
    Guid Create(UserDetailModel userModel);
    Guid? Update(UserDetailModel userModel, string? ownerId);
    void Delete(Guid id, string? ownerId);
}