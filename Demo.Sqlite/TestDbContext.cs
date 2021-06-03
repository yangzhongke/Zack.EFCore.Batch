using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    class TestDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "Data Source=d:\\blogging.db";
            optionsBuilder.UseSqlite(connStr);
            optionsBuilder.UseBatchEF_Sqlite();         
        }
    }
}
