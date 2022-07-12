using Demo.Base;
using Demo.PostgreSQL.Npgsql.heggi;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            //fix: ef core Cannot apply binary operation on types 'timestamp with time zone' and 'timestamp without time zone', convert one of the operands first.”
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            using (TestDbContext ctx = new TestDbContext())
            {
                await ctx.BatchUpdate<Book>()
                    .Set("Title", "Haha")
                    .Set("Price", 3.14)
                    .Set(b => b.PubTime, b => b.PubTime.Value.AddDays(5))
                    .Where(b => b.Price > 888)
                    .ExecuteAsync();

                string title = "666";
                await ctx.BatchUpdate<Book>()
                    .Set(b => b.Title, b => title + b.AuthorName)
                    .Set(b => b.AuthorName, b => title + b.AuthorName)
                    .Set(b => b.Price, b => b.Price + 1)
                    .Where(b => b.Id <= 2)
                    .ExecuteAsync();

                await TestCaseLimit.RunAsync(ctx);
                await TestCase1.RunAsync(ctx);
                ctx.Books.Where(b => b.PubTime == DateTime.Now).ToArray();

                await ctx.BatchUpdate<Book>()
                .Set(b => b.PubTime, b => DateTime.Now)
                .Set(b => b.Title, b => null)
                .Where(b => b.Id > 3)
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
                await TestCaseLimit.RunAsync(ctx);
                await ctx.Comments.Where(c => c.Article.Id == 3).Take(10)
                    .DeleteRangeAsync(ctx);
                
                List<Book> books = TestBulkInsert1.BuildBooks();
                ctx.BulkInsert(books);
                
                List<Author> authors = TestBulkInsert1.BuildAuthors();
                ctx.BulkInsert(authors);

                var articles = TestOwnedType.BuildArticlesForInsert();
                ctx.BulkInsert(articles);
            }
            using (var ctx = new AppDbContext())
            {
                
                /*
                User u = new User();
                u.Uid = Guid.NewGuid().ToString();
                u.Status = SessStatus.Stopping;
                db.Add(u);
                db.SaveChanges();*/
                /*
                ctx.BatchUpdate<User>()
                  .Set(m => m.Status, m => SessStatus.Stopreq) // '1' in DB, must be 'stopreq'
                  //.Set(m=>m.Status,m=>m.Uid=="a"?SessStatus.Stopreq:SessStatus.Stopping) //not supported. Only assignment of constant values to enumerated types is supported currently.
                  .Set(m=>m.Uid,m=>m.Uid+"1")
                  .Where(m=>m.Status== SessStatus.Active)
                  .Execute();*/
                //var u = ctx.User.Select(u=>new { u.Status, b=SessStatus.Active }).FirstOrDefault();
            }
            /*
            var db = new AppDbContext();
            var ip = IPAddress.Parse("1.0.0.1");

            // This throw Exception
            var user = db.User
                .Where(m => EF.Functions.ContainsOrEqual(m.IPv4.Value, ip))
                .FirstOrDefault();

            db.BatchUpdate<User>().Set(b => b.Uid, b => b.Uid + 3)
                .Where(m => EF.Functions.ContainsOrEqual(m.IPv4.Value, ip))
                .Execute();*/
        }
    }
}
