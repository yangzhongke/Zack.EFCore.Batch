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
            string connStr = "Host=192.168.56.101;Database=test;Username=postgres;Password=dLLikhQWy5TBz1uM;Keepalive=30";
            optionsBuilder.UseNpgsql(connStr);
            optionsBuilder.UseBatchEF_Npgsql();
        }
    }
}
