using CsgoClubEF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace CsgoClubEF.Repository
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {


        protected DbContext _context;
        protected DbSet<TEntity> _dbSet;

        public Repository(DbContext context)
        {
            _context = context;
            _dbSet = _context.Set<TEntity>();
        }

        public virtual IQueryable<TEntity> GetAll() => _dbSet;
        public virtual IQueryable<TEntity> GetAllAsNoTracking() => _dbSet.AsNoTracking();

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter)
        {
            IQueryable<TEntity> query = _dbSet.Where(filter);

            return query.ToList();
        }

        public virtual TEntity GetById(Guid id) => GetAll().Where(x => x.Id == id).FirstOrDefault();

        public virtual TEntity GetByIdAsNoTracking(Guid id) => GetAllAsNoTracking().Where(x => x.Id == id).FirstOrDefault();

        public virtual TEntity GetByKey(Expression<Func<TEntity, bool>> filter) => GetAll().Where(filter).FirstOrDefault();


        public virtual IQueryable<TEntity> Query(Expression<Func<TEntity, bool>> filter) => GetAll().Where(filter).AsQueryable();
        public virtual IQueryable<TEntity> QueryAsNoTracking(Expression<Func<TEntity, bool>> filter) => GetAllAsNoTracking().Where(filter).AsQueryable();


        public bool Exists(Guid id) => _dbSet.Any(e => e.Id == id);
        public virtual bool Exists(Expression<Func<TEntity, bool>> filter) => GetAllAsNoTracking().Where(filter).Any();

        public void AttachState(TEntity entity) => _context.Entry(entity).State = EntityState.Unchanged;
        public void Attach(TEntity entity) => _dbSet.Attach(entity);

        public void Detached(TEntity entity) => _context.Entry(entity).State = EntityState.Detached;

        public int Count() => _dbSet.AsNoTracking().Count();

        public virtual void Add(TEntity entity)
        {

            GenerateId(entity);
            _dbSet.Add(entity);
        }

        public virtual void AddMany(IEnumerable<TEntity> entities)
        {

            foreach (var entity in entities)
            {
                GenerateId(entity);
            }
            _dbSet.AddRange(entities);
        }

        public virtual void Remove(Guid id)
        {
            TEntity entityToDelete = _dbSet.Find(id);

            Remove(entityToDelete);
        }

        public virtual void Remove(TEntity entityToDelete)
        {

            GaranteeAttached(entityToDelete);

            _dbSet.Remove(entityToDelete);
        }

        public void RemoveMany(IEnumerable<TEntity> entities)
        {
            entities.ToList().ForEach(e =>
            {
                GaranteeAttached(e);
            });
            _dbSet.RemoveRange(entities);
        }

        private void GaranteeAttached(TEntity e)
        {
            if (_context.Entry(e).State == EntityState.Detached)
                _dbSet.Attach(e);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            _dbSet.Attach(entityToUpdate);
            _context.Entry(entityToUpdate).State = EntityState.Modified;
        }

        private static void GenerateId(TEntity entity)
        {
            if (entity.Id == null || Guid.Empty == entity.Id)
                entity.Id = Guid.NewGuid();
        }
    }
}
