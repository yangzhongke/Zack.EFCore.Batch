using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Demo
{
    public class TestDbContext : BaseDbContext
    {
        public TestDbContext()
        {

        }

        public TestDbContext(DbContextOptions<TestDbContext> options) :
            base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.LogTo(Console.WriteLine);
            
            string connStr = "server=localhost;user=root;password=root;database=ef;AllowLoadLocalInfile=true";
            optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(8, 0, 20)),builder=> {
                builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
            });
#if (!NET7_0_OR_GREATER)
            optionsBuilder.UseBatchEF_MySQLPomelo();
#endif
        }
    }
}
