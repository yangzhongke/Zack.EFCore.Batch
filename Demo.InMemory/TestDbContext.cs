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
            optionsBuilder.UseInMemoryDatabase(databaseName: "Test");
            optionsBuilder.UseBatchEF_InMemory();            
        }
    }
}
