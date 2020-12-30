using Microsoft.EntityFrameworkCore;
using System;

namespace Demo.PostgreSQL.Npgsql.heggi
{
    public class AppDbContext : DbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine);
            optionsBuilder.UseNpgsql("Host=127.0.0.1;Database=test;Username=postgres;Password=123456;Keepalive=30");
            optionsBuilder.UseBatchEF_Npgsql();
        }

        public DbSet<User> User { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder
                .Entity<User>()
                .Property(m => m.Status)
                .HasConversion(new CustomEnumConverter<SessStatus>());
        }
    }
}
