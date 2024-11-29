using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Forms.Common;
using Forms.Web.DAL.Repositories.Interfaces;

namespace Forms.Web.DAL.Repositories
{
    public abstract class RepositoryBase<T> : IWebRepository<T>
        where T : IWithId
    {
        private readonly LocalDb _localDb;
        public abstract string TableName { get; }

        protected RepositoryBase(LocalDb localDb)
        {
            this._localDb = localDb;
        }

        public async Task<IList<T>> GetAllAsync()
        {
            return await _localDb.GetAllAsync<T>(TableName);
        }

        public async Task<T> GetByIdAsync(Guid id)
        {
            return await _localDb.GetByIdAsync<T>(TableName, id);
        }

        public async Task InsertAsync(T entity)
        {
            await _localDb.InsertAsync(TableName, entity);
        }

        public async Task RemoveAsync(Guid id)
        {
            await _localDb.RemoveAsync(TableName, id);
        }
    }
}