using CsgoClubEF.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CsgoClubEF.Repository
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<GameMatch>();
            builder.Entity<Server>();
            builder.Entity<User>().HasMany(p=> p.Friends).WithOne(p=> p.User).OnDelete(DeleteBehavior.NoAction);
            builder.Entity<FriendList>();
            builder.Entity<PlayerToMatch>();
            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {

            var entities = ChangeTracker.Entries().Where(x => x.Entity is BaseEntity);

            entities.Where(x => x.State == EntityState.Added).ToList().ForEach(entity => {
                var entityTemp = ((BaseEntity)entity.Entity);

                if (entityTemp.Id == null || entityTemp.Id == Guid.Empty)
                    entityTemp.Id = Guid.NewGuid();
            });
            return base.SaveChanges();
        }
    }
}
