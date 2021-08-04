using Demo.Base.SyncApi;
using Microsoft.EntityFrameworkCore;

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
