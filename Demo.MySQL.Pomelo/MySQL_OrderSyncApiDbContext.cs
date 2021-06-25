using Demo.Base.SyncApi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.MySQL.Pomelo
{
    public class MySQL_OrderSyncApiDbContext: OrderSyncApiDbContext
    {
        public MySQL_OrderSyncApiDbContext(DbContextOptions<MySQL_OrderSyncApiDbContext> options)
            : base(options)
        {
        }
    }
}
