using Forms.Common.BL.Facades;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.Facades;

public interface IFormFacade : IAppFacade
{
    List<FormListModel> GetAll();
    FormDetailModel? GetById(Guid id);
    Guid CreateOrUpdate(FormDetailModel formModel, string? ownerId = null);
    Guid Create(FormDetailModel formModel, string? ownerId = null);
    Guid? Update(FormDetailModel formModel, string? ownerId = null);
    void Delete(Guid id, string? ownerId = null);
}