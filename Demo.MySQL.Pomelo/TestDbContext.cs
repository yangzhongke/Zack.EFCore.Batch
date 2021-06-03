using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace Demo
{
    class TestDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "server=localhost;user=root;password=root;database=ef";
            optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)),
                        mySqlOptions => mySqlOptions
                            .CharSetBehavior(CharSetBehavior.NeverAppend));

            optionsBuilder.UseBatchEF_MySQLPomelo();            
        }
    }
}
