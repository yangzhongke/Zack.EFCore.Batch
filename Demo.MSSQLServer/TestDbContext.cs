using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    class TestDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "Server=.;Database=demoBatch;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            optionsBuilder.UseBatchEF_MSSQL();            
        }
    }
}
