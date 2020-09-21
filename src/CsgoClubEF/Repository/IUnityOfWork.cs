using CsgoClubEF.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace CsgoClubEF.Repository
{
    public interface IUnityOfWork : IDisposable
    {
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;

        void Save();
    }
}
