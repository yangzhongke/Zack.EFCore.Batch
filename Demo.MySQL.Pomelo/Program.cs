using Demo.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.Extensions.DependencyInjection;
using Demo.MySQL.Pomelo;
using System;
using Microsoft.EntityFrameworkCore;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            ServiceCollection services = new ServiceCollection();
            services.AddDbContextPool<PooledTestDbContext>(optionsBuilder => {
                optionsBuilder.LogTo(Console.WriteLine);
                string connStr = "server=localhost;user=root;password=root;database=ef;AllowLoadLocalInfile=true";
                optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)), builder => {
                    builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                });
                optionsBuilder.UseBatchEF_MySQLPomelo();
            });
            using (var sp = services.BuildServiceProvider())
            using (var ctx = sp.GetService<PooledTestDbContext>())
            {
                await TestCaseLimit.RunAsync(ctx);
                await TestCase1.RunAsync(ctx);
                ctx.Books.Where(b=>b.PubTime==ctx.Now(6)).ToArray();

                await ctx.BatchUpdate<Book>()
                .Set(b => b.PubTime, b => ctx.Now(6))
                //.Where(b => b.Id >3)
                .ExecuteAsync();
            }
            /*
            using (TestDbContext ctx = new TestDbContext())
            {
                await TestCaseLimit.RunAsync(ctx);
            }*/
        }
    }
}
