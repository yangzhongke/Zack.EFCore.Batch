using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    class TestDbContext : BaseDbContext
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "Host=127.0.0.1;Database=test;Username=postgres;Password=123456;Keepalive=30";
            optionsBuilder.UseNpgsql(connStr);
            optionsBuilder.UseBatchEF_Npgsql();
        }
    }
}
