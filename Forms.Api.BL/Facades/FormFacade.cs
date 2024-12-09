using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Forms.Common.Models.Form;

namespace Forms.Api.BL.Facades;

public class FormFacade : FacadeBase<IFormRepository, FormEntity>, IFormFacade
{
    private readonly IMapper mapper;
    private readonly IFormRepository formRepository;


    public FormFacade(IFormRepository formRepository, IMapper mapper) 
        : base(formRepository) 
    {
        this.mapper = mapper;
        this.formRepository = formRepository;
    }
    
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

    public Guid CreateOrUpdate(FormDetailModel formModel, string? ownerId = null)
    {
        return formRepository.Exists(formModel.Id)
            ? Update(formModel, ownerId)!.Value
            : Create(formModel, ownerId);
    }

    public Guid Create(FormDetailModel formModel, string? ownerId = null)
    {
        var formEntity = mapper.Map<FormEntity>(formModel);
        formEntity.OwnerId = ownerId;
        
        return formRepository.Insert(formEntity);
    }

    public Guid? Update(FormDetailModel formModel, string? ownerId = null)
    {
        ThrowIfWrongOwner(formModel.Id, ownerId);
        
        var formEntity = mapper.Map<FormEntity>(formModel);
        return formRepository.Update(formEntity);
    }

    public void Delete(Guid id, string? ownerId = null)
    {
        ThrowIfWrongOwner(id, ownerId);

        
        formRepository.Remove(id);
    }
}