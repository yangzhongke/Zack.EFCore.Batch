using Microsoft.EntityFrameworkCore;
using System;

namespace Demo
{
    class TestDbContext : BaseDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(msg=> { 
                if(msg.Contains("QueryExecutionPlanned"))
                {
                    Console.WriteLine(msg);
                }
            });
            string connStr = "Server=.;Database=demoBatch;Trusted_Connection=True;MultipleActiveResultSets=true";
            optionsBuilder.UseSqlServer(connStr);
            optionsBuilder.UseBatchEF_MSSQL();            
        }
    }
}
