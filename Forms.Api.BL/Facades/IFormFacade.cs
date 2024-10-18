using Forms.Common.BL.Facades;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.Facades;

public interface IFormFacade : IAppFacade
{
    List<FormListModel> GetAll();
    FormDetailModel? GetById(Guid id);
    Guid CreateOrUpdate(FormDetailModel formModel);
    Guid Create(FormDetailModel formModel);
    Guid? Update(FormDetailModel formModel);
    void Delete(Guid id);
}