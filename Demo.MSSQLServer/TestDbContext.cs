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
            string connStr = "Server=.;Database=demoBatch;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
            optionsBuilder.UseSqlServer(connStr, x => x.UseNodaTime());
#if (!NET7_0_OR_GREATER)
            optionsBuilder.UseBatchEF_MSSQL();     
#endif

        }
    }
}
