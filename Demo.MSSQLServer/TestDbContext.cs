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
        var connStr =
            "Server=.;Database=demoBatch;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";
        optionsBuilder.UseSqlServer(connStr, x => x.UseNodaTime());
    }
}