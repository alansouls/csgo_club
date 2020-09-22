using CsgoClubEF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CsgoClubEF.Repository
{
    public class UnitOfWork : IDisposable, IUnityOfWork
    {
        private readonly Context _context;
        private readonly Hashtable repositories;
        private bool disposed = false;

        public UnitOfWork(Context context)
        {
            _context = context;
            repositories = new Hashtable();
        }
        
        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity
        {
            if (disposed)
                throw new ObjectDisposedException("Unit Of Work");
            if (repositories.Contains(typeof(TEntity)))
                return repositories[typeof(TEntity)] as IRepository<TEntity>;

            var newRepository = new Repository<TEntity>(_context);
            repositories.Add(typeof(TEntity), newRepository);

            return newRepository;
        }

        public void Save()
        {
            if (disposed)
                throw new ObjectDisposedException("Unit Of Work");

            _context.SaveChanges();
        }

        public void Dispose() 
        {
            disposed = true;
            _context.Dispose();
        }
    }
}
