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
            string connStr = "SERVER=172.29.82.96;PORT=5236;USER=SYSDBA;PASSWORD=SYSDBASYSDBA";
            optionsBuilder.UseDm(connStr);
            optionsBuilder.UseBatchEF_DM();            
        }
    }
}
