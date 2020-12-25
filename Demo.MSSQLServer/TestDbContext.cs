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
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "Server=.;Database=demo1;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            optionsBuilder.UseBatchEF_MSSQL();            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        }
    }
}
