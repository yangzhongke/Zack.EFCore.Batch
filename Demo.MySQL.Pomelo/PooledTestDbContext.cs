using Microsoft.EntityFrameworkCore;
using System;

namespace Demo.MySQL.Pomelo
{
    class PooledTestDbContext : BaseDbContext
    {
        public PooledTestDbContext(DbContextOptions<PooledTestDbContext> options):
            base(options)
        {
            
        }

        public DateTime Now(int prec) => throw new NotSupportedException();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.HasDbFunction(typeof(PooledTestDbContext).GetMethod(nameof(PooledTestDbContext.Now), new[] { typeof(int) })).IsBuiltIn();
        }
    }
}
