using Forms.Common.BL.Facades;
using Forms.Common.Models.User;

namespace Forms.Api.BL.Facades;

public interface IUserFacade : IAppFacade
{
    List<UserListModel> GetAll();
    UserDetailModel? GetById(Guid id);
    Guid CreateOrUpdate(UserDetailModel recipeModel);
    Guid Create(UserDetailModel recipeModel);
    Guid? Update(UserDetailModel recipeModel);
    void Delete(Guid id);
}