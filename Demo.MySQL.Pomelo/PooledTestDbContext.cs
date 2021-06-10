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
    }
}
