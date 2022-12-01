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
#if (!NET7_0_OR_GREATER)
            optionsBuilder.UseBatchEF_Sqlite(); 
#endif
        }
    }
}
