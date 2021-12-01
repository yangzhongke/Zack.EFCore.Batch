using Demo.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connStr = "server=localhost;user=root;password=adfa3_ioz09_08nljo;database=zackbatch;AllowLoadLocalInfile=true";
            ServiceCollection services = new ServiceCollection();
            /*
            services.AddDbContext<TestDbContext>(optionsBuilder =>
            {
                optionsBuilder.LogTo(Console.WriteLine);
                optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)), builder =>
                {
                    builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                });
                optionsBuilder.UseBatchEF_MySQLPomelo();
            });*/
            services.AddDbContext<TestDbContext>();
            using (var sp = services.BuildServiceProvider())
            {
                using (TestDbContext ctx = sp.GetRequiredService<TestDbContext>())
                {
                    string title = "666";
                    await ctx.BatchUpdate<Book>()
                        .Set(b => b.Title, b => title+b.AuthorName)
                        .Set(b=>b.AuthorName,b=>title + b.AuthorName)
                        .Set(b=>b.Price,b=>b.Price+1)
                        .Where(b => b.Id <= 2)
                        .ExecuteAsync();
                }
            }
        }
        static async Task Main2(string[] args)
        {
            string connStr = "server=localhost;user=root;password=adfa3_ioz09_08nljo;database=zackbatch;AllowLoadLocalInfile=true";
            ServiceCollection services = new ServiceCollection();
            services.AddDbContextPool<PooledTestDbContext>(optionsBuilder =>
            {
                optionsBuilder.LogTo(Console.WriteLine);
                optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)), builder =>
                {
                    builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                });
                optionsBuilder.UseBatchEF_MySQLPomelo();
            });
            services.AddDbContext<MySQL_OrderSyncApiDbContext>(optionsBuilder =>
            {
                optionsBuilder.LogTo(Console.WriteLine);
                optionsBuilder.UseMySql(connStr, new MySqlServerVersion(new Version(5, 6, 20)), builder =>
                {
                    builder.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                });
                optionsBuilder.UseBatchEF_MySQLPomelo();
            });
            services.AddDbContext<Issue24Context>();
            using (var sp = services.BuildServiceProvider())
            using (var ctx = sp.GetService<PooledTestDbContext>())
            using (var orderCtx = sp.GetService<MySQL_OrderSyncApiDbContext>())
            using (var issue24Ctx = sp.GetService<Issue24Context>())
            {
                string eva = null;
                issue24Ctx.BatchUpdate<Demo.Base.Issue24.Book>()
                    .Set(b=>b.AuthorName, b=>null)
                    .Execute();
                issue24Ctx.BatchUpdate<Demo.Base.Issue24.Book>()
                    .Set(b => b.AuthorName, b => eva)
                    .Execute();
                /*
                issue24Ctx.BulkInsert(TestCaseIssue24.BuildData());
                
                await TestCaseLimit.RunAsync(ctx);
                await TestCase1.RunAsync(ctx);
                ctx.Books.Where(b => b.PubTime == ctx.Now(6)).ToArray();
                
                await ctx.BatchUpdate<Book>()
                .Set(b => b.PubTime, b => ctx.Now(6))
                .Set(b=>b.Title,b=>null)
                //.Where(b => b.Id >3)
                .ExecuteAsync();
                
                string platformOrderNo = "666";
                
                await orderCtx.PlatformOrderInfos.DeleteRangeAsync(orderCtx, x => x.PlatformOrderNo == platformOrderNo);
                
                await orderCtx.DeleteRangeAsync<PlatformOrderInfo>(x => x.PlatformOrderNo == platformOrderNo);

                //issue: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/18
                await orderCtx.BatchUpdate<PlatformOrderInfo>()
                    .Where(x => x.PlatformOrderNo == platformOrderNo)
                    //.Set(x => x.State, x => 10)//error
                    //.Set(x => x.State, x => (sbyte)10)//OK
                    .Set<sbyte>(x => x.State, x => 10)//OK
                    .ExecuteAsync();

                *、
            }
            /*
            using (TestDbContext ctx = new TestDbContext())
            {
                await TestCaseLimit.RunAsync(ctx);
            }*/
            }
        }
    }
}
