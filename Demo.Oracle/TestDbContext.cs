using Microsoft.EntityFrameworkCore;
using System;
using Zack.EFCore.Batch.Oracle;

namespace Demo
{
    class TestDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "User Id=C##yzk;Password=Hello123456;Data Source=192.168.142.128:1521/XE";
            optionsBuilder.UseOracle(connStr);
            optionsBuilder.UseBatchEF_Oracle();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        }
    }
}
