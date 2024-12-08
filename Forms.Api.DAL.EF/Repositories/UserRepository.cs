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
    
    public virtual void Remove(Guid id)
    {
        var entity = GetById(id);

        if (entity is not null)
        {
            Console.WriteLine($"Deleting entity with ID: {id}");

            // Explicitní odstranění navázaných entit
            DeleteRelatedEntities(id);
            context.SaveChanges();

            context.Users.Remove(entity);

            try
            {
                context.SaveChanges();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                Console.WriteLine($"Concurrency exception: {ex.Message}");
                throw;
            }
        }
        else
        {
            throw new KeyNotFoundException($"Entity with ID {id} not found.");
        }
    }

    private void DeleteRelatedEntities(Guid userId)
    {
        // Smazání všech formulářů přidružených k uživateli
        var forms = context.Set<FormEntity>().Where(f => f.UserId == userId).ToList();
        if (forms.Any())
        {
            Console.WriteLine($"Deleting {forms.Count} related forms.");
            context.Set<FormEntity>().RemoveRange(forms);
        }
        
    }


    public override UserEntity? GetById(Guid id)
    {
        return context.Users
            .Include(u => u.Responses) 
            .Include(u => u.Forms)
            .ThenInclude(f => f.Questions)
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


    public async Task<List<UserEntity>> SearchAsync(string query)
    {
        query = query.ToLower();

        return await context.Users
            .Where(u => 
                u.FirstName.ToLower().Contains(query) || 
                u.LastName.ToLower().Contains(query) ||
                u.Email.ToLower().Contains(query))
            .ToListAsync();
    }
}