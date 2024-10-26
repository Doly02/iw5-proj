using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.DAL.EF.Repositories;

public class FormRepository : RepositoryBase<FormEntity>, IFormRepository
{
    private readonly IMapper _mapper;
    
    public FormRepository(FormsDbContext context, IMapper mapper) : base(context)
    {
        this._mapper = mapper;
    }

    public override FormEntity? GetById(Guid id)
    {
        return context.Forms
            .Include(f => f.User)
            .Include(f => f.Questions)
            .FirstOrDefault(f => f.Id == id);
    }
    
    
    public override Guid? Update(FormEntity form)
    {
        if (Exists(form.Id))
        {
            var existingForm = context.Forms
                .Include(f => f.User)
                .Include(f => f.Questions)
                .Single(u => u.Id == form.Id);
            
            _mapper.Map(form, existingForm);
            
            context.Forms.Update(existingForm);
            context.SaveChanges();
            
            return existingForm.Id;
        }

        return null;
    }
}