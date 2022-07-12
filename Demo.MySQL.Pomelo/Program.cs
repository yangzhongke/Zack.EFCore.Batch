using Demo.Base;
using Demo.Base.Issue24;
using Demo.Base.SyncApi;
using Demo.MySQL.Pomelo;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string connStr = "server=localhost;user=root;password=root;database=zackbatch;AllowLoadLocalInfile=true";
            ServiceCollection services = new ServiceCollection();
            services.AddDbContext<TestDbContext>();
            using (var sp = services.BuildServiceProvider())
            {
                using (TestDbContext ctx = sp.GetRequiredService<TestDbContext>())
                {
                    await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
                        .Where(c => c.Id == 3)
                        .Skip(3)
                        .ExecuteAsync();
                    await ctx.Comments.Where(c => c.Id > 3).Skip(5).Take(10)
                         .DeleteRangeAsync(ctx);
                    await ctx.Comments.Where(c => c.Article.Id == 3).OrderBy(c => c.Message).Skip(5).Take(10)
                        .DeleteRangeAsync(ctx);

                    string title = "666";
                    await ctx.BatchUpdate<Book>()
                        .Set(b => b.Title, b => title+b.AuthorName)
                        .Set(b=>b.AuthorName,b=>title + b.AuthorName)
                        .Set(b=>b.Price,b=>b.Price+1)
                        .Where(b => b.Id <= 2)
                        .ExecuteAsync();

                    await TestCaseLimit.RunAsync(ctx);
                    await TestCase1.RunAsync(ctx);
                    ctx.Books.Where(b => b.PubTime == DateTime.Now).ToArray();

                    await ctx.BatchUpdate<Book>()
                    .Set(b => b.PubTime, b => DateTime.Now)
                    .Set(b => b.Title, b => null)
                    .Where(b => b.Id >3)
                    .ExecuteAsync();

                    title = "zack";
                    await ctx.BatchUpdate<Book>()
                        .Set(b => b.Title, b => title)
                        .Where(b => b.Id <= 2)
                        .ExecuteAsync();
                    await ctx.BatchUpdate<Book>()
                    .Set(a => a.Title, a => "测试")
                    .Set(a => a.AuthorName, a => a.AuthorName)
                    .Where(a => ctx.Articles.Where(b => b.Id == a.Id && b.Content == "B").Any())
                    .ExecuteAsync();
                    await TestCase1.RunAsync(ctx);
                    await TestCase2.RunAsync(ctx);
                    List<Book> books = TestBulkInsert1.BuildBooks();
                    ctx.BulkInsert(books);
                    List<Author> authors = TestBulkInsert1.BuildAuthors();
                    ctx.BulkInsert(authors);
                    var articles = TestOwnedType.BuildArticlesForInsert();
                    ctx.BulkInsert(articles);
                    await TestCaseLimit.RunAsync(ctx);
                    await ctx.Comments.Where(c => c.Article.Id == 3).Take(10)
                        .DeleteRangeAsync(ctx);
                    await ctx.BatchUpdate<Book>()
                    .Set("Title", "Haha")
                    .Set("Price", 3.14)
                    .Set(b=>b.PubTime,DateTime.Now)
                    .Where(b => b.Price > 888)
                    .ExecuteAsync();
                    await ctx.BatchUpdate<Book>()
                    .Set("Title", "Haha")
                    .Set("Price", 3)
                    .Where(b => b.Price > 888)
                    .ExecuteAsync();
                    
                }
            }
        }
        static async Task Main2(string[] args)
        {
            string connStr = "server=localhost;user=root;password=root;database=zackbatch;AllowLoadLocalInfile=true";
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
            {              
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
            
            }
        }
    }
}
