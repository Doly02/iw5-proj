using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.DAL.EF.Repositories;

public class ResponseRepository : RepositoryBase<ResponseEntity>, IResponseRepository
{
    private readonly IMapper _mapper;
    
    public ResponseRepository(FormsDbContext context, IMapper mapper) : base(context)
    {
        this._mapper = mapper;
    }

    public override ResponseEntity? GetById(Guid id)
    {
        return context.Responses
            .FirstOrDefault(r => r.Id == id); 
    }

    public override Guid? Update(ResponseEntity response)
    {
        if (Exists(response.Id))
        {
            var existingResponse = context.Responses
                .Single(u => u.Id == response.Id);
            
            _mapper.Map(response, existingResponse);
            
            context.Responses.Update(existingResponse);
            context.SaveChanges();
            
            return existingResponse.Id;
        }
        else
        {
            return null;
        }
    }
    
    
}