using Microsoft.EntityFrameworkCore;
using System;

namespace Demo.Base.SyncApi
{
    public class OrderSyncApiDbContext : DbContext
    {
        /* Add DbSet for each Aggregate Root here. Example:
         * public DbSet<Question> Questions { get; set; }
         */

        public DbSet<PlatformOrderInfo> PlatformOrderInfos { get; set; }

        public OrderSyncApiDbContext(DbContextOptions options)
            : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ConfigureOrderSyncApi();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);

            //optionsBuilder.UseBatchEF_MySQLPomelo();//as for MySQL

            //base.OnConfiguring(optionsBuilder);
        }
    }
}
