namespace Forms.Api.DAL.Common.Repositories;

public interface ISearchRepository<TEntity>
{
    Task<List<TEntity>> SearchAsync(string query);
}
