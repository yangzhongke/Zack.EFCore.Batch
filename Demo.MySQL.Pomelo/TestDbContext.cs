using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
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
            string connStr = "server=localhost;user=root;password=root;database=ef";
            optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)),
                        mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend));

            optionsBuilder.UseBatchEF_MySQLPomelo();            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        }
    }
}
