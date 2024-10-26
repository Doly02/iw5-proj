using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.Memory.Repositories
{
    public class FormRepository : IFormRepository
    {
        private readonly IList<FormEntity> _forms;
        private readonly IMapper _mapper;

        public FormRepository(
            Storage storage,
            IMapper mapper)
        {
            _forms = storage.Forms;
            _mapper = mapper;
        }

        public IList<FormEntity> GetAll()
        {
            return _forms;
        }

        public FormEntity? GetById(Guid id)
        {
            return _forms.SingleOrDefault(entity => entity.Id == id);
        }

        public Guid Insert(FormEntity form)
        {
            _forms.Add(form);
            return form.Id;
        }

        public Guid? Update(FormEntity entity)
        {
            var formExists = _forms.SingleOrDefault(formInStore =>
                formInStore.Id == entity.Id);
            if (null != formExists) {
                _mapper.Map(entity, formExists);
            }
            return formExists?.Id;
        }

        public void Remove(Guid id)
        {
            var formToRemove = _forms.Single(form => form.Id.Equals(id));
            _forms.Remove(formToRemove);
        }

        public bool Exists(Guid id)
        {
            return _forms.Any(form => form.Id == id);
        }
    }
}