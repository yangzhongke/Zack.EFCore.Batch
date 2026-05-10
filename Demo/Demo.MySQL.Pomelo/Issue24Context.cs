using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Demo.MySQL.Pomelo;

public class Issue24Context : DbContext
{
    public DbSet<Base.Issue24.Book> Books { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.LogTo(Console.WriteLine);
        var connStr = "server=localhost;user=root;password=root;database=zackbatch;AllowLoadLocalInfile=true";
        optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)),
            builder => { builder.SchemaBehavior(MySqlSchemaBehavior.Ignore); });
    }
}