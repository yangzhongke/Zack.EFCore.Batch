using Demo.DM_NET6;
using Microsoft.EntityFrameworkCore;

namespace Demo;

internal class TestDbContext : BaseDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(msg =>
        {
            if (msg.Contains("QueryExecutionPlanned")) Console.WriteLine(msg);
        });
        optionsBuilder.UseDm(SQLHelper.ConnStr);
    }
}