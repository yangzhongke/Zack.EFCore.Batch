using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;

namespace Demo.MySQL.Pomelo
{
    public class Issue24Context: DbContext
    {
        public DbSet<Demo.Base.Issue24.Book> Books { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            string connStr = "server=localhost;user=root;password=root;database=zackbatch;AllowLoadLocalInfile=true";
            optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)), builder => {
                builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
#if (!NET7_0_OR_GREATER)
            optionsBuilder.UseBatchEF_MySQLPomelo();
#endif
        }
    }
}
