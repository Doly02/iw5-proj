using Forms.Common.BL.Facades;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.Facades;

public interface IFormFacade : IAppFacade
{
    List<FormListModel> GetAll();
    FormDetailModel? GetById(Guid id);
    Guid CreateOrUpdate(FormDetailModel recipeModel);
    Guid Create(FormDetailModel recipeModel);
    Guid? Update(FormDetailModel recipeModel);
    void Delete(Guid id);
}