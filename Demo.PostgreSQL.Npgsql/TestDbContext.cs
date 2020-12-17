using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    class TestDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "Host=127.0.0.1;Database=test;Username=postgres;Password=123456;Keepalive=30";
            optionsBuilder.UseNpgsql(connStr);
            optionsBuilder.UseBatchEF_Npgsql();
            //optionsBuilder.UseBatchEF();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        }
    }
}
