using Microsoft.EntityFrameworkCore;

namespace Demo;

internal class TestDbContext : BaseDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
        var connStr = "User Id=C##BatchTestUser;Password=123456;Data Source=192.168.1.185:1521/FREE";
        optionsBuilder.UseOracle(connStr);
    }
}