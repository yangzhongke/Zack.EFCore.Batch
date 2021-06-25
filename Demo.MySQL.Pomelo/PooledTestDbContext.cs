using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
