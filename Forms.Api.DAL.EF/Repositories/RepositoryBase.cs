using Forms.Api.DAL.Common.Entities.Interfaces;
using Forms.Api.DAL.Common.Repositories;

namespace Forms.Api.DAL.EF.Repositories;

public class RepositoryBase<TEntity> : IApiRepository<TEntity>, IDisposable
    where TEntity : class, IEntity
{
    protected readonly FormsDbContext context;

    protected RepositoryBase(FormsDbContext context)
    {
        this.context = context;
    }

    public virtual IList<TEntity> GetAll()
    {
        return context.Set<TEntity>().ToList();
    }

    public virtual TEntity? GetById(Guid id)
    {
        return context.Set<TEntity>().SingleOrDefault(x => x.Id == id);
    }

    public virtual Guid Insert(TEntity entity)
    {
        var createdEntity = context.Set<TEntity>().Add(entity);

        context.SaveChanges();

        return createdEntity.Entity.Id;
    }

    public virtual Guid? Update(TEntity entity)
    {
        if (Exists(entity.Id))
        {
            context.Set<TEntity>().Attach(entity);
            var updatedEntity = context.Set<TEntity>().Update(entity);
            context.SaveChanges();

            return updatedEntity.Entity.Id;
        }

        return null;
    }

    public virtual void Remove(Guid id)
    {
        var entity = GetById(id);
        if (entity is not null)
        {
            context.Set<TEntity>().Remove(entity);
            context.SaveChanges();
        }
    }

    public virtual bool Exists(Guid id)
    {
        return context.Set<TEntity>().Any(entity => entity.Id == id);
    }

    public void Dispose()
    {
        context.Dispose();
    }
}