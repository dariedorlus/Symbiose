using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Symbiose.Mail.Data;

namespace Symbiose.Mail.Repositories
{
    public interface IRepository<T>
    {
        /// <summary>
        /// Gets all.
        /// </summary>
        /// <returns></returns>
        Task<IQueryable<T>> GetAll();

        /// <summary>
        /// Gets an entity <permission cref="T"></permission>>
        /// </summary>
        /// <param name="id">The id of the entity to be retrieved </param>
        /// <returns>The entity</returns>
        Task<T> GetOneById(string id);


        /// <summary>Create a new entity in storage.</summary>
        /// <param name="entity">The entity</param>
        /// <returns>
        /// The newly created entity
        ///   <br />
        /// </returns>
        Task<T> InsertOneEntity(T entity);
    }


    public abstract class RepositoryBase<TEntity>: IRepository<TEntity> 
        where TEntity: class 
    {
        private readonly DbSet<TEntity> dbSet;

        protected RepositoryBase(AppDbContext dbContext)
        {
             dbSet = dbContext.Set<TEntity>();
        }

        public virtual async Task<IQueryable<TEntity>> GetAll()
        {
            return await Task.FromResult(dbSet.AsQueryable());
        }

        public virtual async Task<TEntity> GetOneById(string Id)
        {
            var res = await dbSet.FindAsync(Id);
            return res;
        }

        public virtual async Task<TEntity> InsertOneEntity(TEntity entity)
        {
            var res = await dbSet.AddAsync(entity);
            return res.Entity;
        }
    }
}
