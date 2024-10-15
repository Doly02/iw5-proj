using AutoMapper;
using Forms.Api.DAL.Common.Entities;
using Forms.Api.DAL.Common.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Forms.Api.DAL.EF.Repositories;

public class UserRepository : RepositoryBase<UserEntity>, IUserRepository
{
    private readonly IMapper mapper;
    
    public UserRepository(FormsDbContext context, IMapper mapper) : base(context)
    {
        this.mapper = mapper;
    }

    public override UserEntity? GetById(Guid id)
    {
        return context.Users
            .Include(u => u.Responses) 
            .Include(u => u.Forms)
            .FirstOrDefault(u => u.Id == id); 
    }

    public override Guid? Update(UserEntity user)
    {
        if (Exists(user.Id))
        {
            var existingUser = context.Users
                .Include(u => u.Responses)
                .Include(u => u.Forms)
                .Single(u => u.Id == user.Id);
            
            mapper.Map(user, existingUser);
            
            context.Users.Update(existingUser);
            context.SaveChanges();
            
            return existingUser.Id;
        }

        return null;
    }
    
    
}