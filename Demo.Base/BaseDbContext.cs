using Demo.Base;
using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    public abstract class BaseDbContext : DbContext
    {
        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }

        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(this.GetType().Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
            modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        }
    }
}
