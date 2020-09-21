using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CsgoClubEF.Repository
{
    public interface IRepository<TEntity> where TEntity : BaseEntity
    {
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> filter);

        IQueryable<TEntity> GetAll();
        IQueryable<TEntity> GetAllAsNoTracking();

        TEntity GetById(Guid id);
        TEntity GetByIdAsNoTracking(Guid id);

        TEntity GetByKey(Expression<Func<TEntity, bool>> filter);

        IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter);
        IQueryable<TEntity> QueryAsNoTracking(Expression<Func<TEntity, bool>> filter);

        void Add(TEntity entity);
        void AddMany(IEnumerable<TEntity> entities);

        void Remove(Guid id);
        void Remove(TEntity entity);
        void RemoveMany(IEnumerable<TEntity> entities);

        void Update(TEntity entity);

        bool Exists(Guid id);
        bool Exists(Expression<Func<TEntity, bool>> filter);

        void Attach(TEntity entity);
        void AttachState(TEntity entity);
        int Count();
        void Detached(TEntity entity);
    }
}
