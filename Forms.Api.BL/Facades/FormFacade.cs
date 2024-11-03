using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.Facades;

public class FormFacade(IFormRepository formRepository, IMapper mapper) : IFormFacade
{
    public List<FormListModel> GetAll()
    {
        var formEntities = formRepository.GetAll();
        return mapper.Map<List<FormListModel>>(formEntities);
    }

    public FormDetailModel? GetById(Guid id)
    {
        var formEntity = formRepository.GetById(id);
        return mapper.Map<FormDetailModel>(formEntity);
    }

    public Guid CreateOrUpdate(FormDetailModel formModel)
    {
        return formRepository.Exists(formModel.Id)
            ? Update(formModel)!.Value
            : Create(formModel);
    }

    public Guid Create(FormDetailModel formModel)
    {
        var formEntity = mapper.Map<FormEntity>(formModel);
        return formRepository.Insert(formEntity);
    }

    public Guid? Update(FormDetailModel formModel)
    {
        var formEntity = mapper.Map<FormEntity>(formModel);
        return formRepository.Update(formEntity);
    }

    public void Delete(Guid id)
    {
        formRepository.Remove(id);
    }
}