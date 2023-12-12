using Demo.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using NodaTime;
using System.Diagnostics;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using TestDbContext ctx = new TestDbContext();

			//Instant now = SystemClock.Instance.GetCurrentInstant();                       
			//await ctx.DeleteRangeAsync<NodaTimeEntity>(p => p.Instant <= now, true);
			//await ctx.DeleteRangeAsync<NodaTimeEntity>(p => p.Instant <= SystemClock.Instance.GetCurrentInstant(), true);
#if (!NET7_0_OR_GREATER)
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
            .Where(c => c.Id == 3)
            .Skip(3)
            .ExecuteAsync();
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
                .Where(c => c.Article.Id == 3)
                .Skip(3)
                .ExecuteAsync();
            await ctx.Comments.Where(c => c.Id > 3).Skip(5).Take(10)
                .DeleteRangeAsync(ctx);
            await ctx.Comments.Where(c => c.Article.Id==3).OrderBy(c=>c.Message).Skip(5).Take(10)
                .DeleteRangeAsync(ctx);
            string title = null;
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
            
            await TestCaseLimit.RunAsync(ctx);
            await ctx.Comments.Where(c => c.Article.Id == 3).Take(10)
                .DeleteRangeAsync(ctx);
            await ctx.BatchUpdate<Book>()
            .Set(a => a.AuthorName, a => a.AuthorName)
            .Where(a => ctx.Articles.Where(b => b.Id == a.Id && b.Content == "B").Any())
            .ExecuteAsync();
            await ctx.BatchUpdate<Book>()
            .Set("Title", "Haha")
            .Set("Price", 3.14)
            .Set(b => b.PubTime, DateTime.Now)
            .Where(b => b.Price > 888)
            .ExecuteAsync();
            await ctx.BatchUpdate<Book>()
            .Set("Title", "Haha")
            .Set("Price", 3)
            .Where(b => b.Price > 888)
            .ExecuteAsync();
                       await ctx.BatchUpdate<Book>()
            .Set(b=>b.Price,b=>3)
            .Set(b=>b.Pages,b=>3)
            .Where(b => b.Price > 888)
            .ExecuteAsync();

            await ctx.BatchUpdate<Book>()
				.Set("Title", "Haha")
            .Set("Price", 3.14)
            .Set(b => b.PubTime, DateTime.Now)
            .Where(b => b.Price > 888)
            .ExecuteAsync();
#endif
			List<Author> authors = TestBulkInsert1.BuildAuthors();
			ctx.BulkInsert(authors);
			ctx.BulkInsert(TestOwnedType.BuildArticlesForInsert());
			
            var items = TestOwnedType.BuildArticlesForInsert(100);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ctx.BulkInsert(items);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            stopwatch.Reset();
            stopwatch.Start();
            ctx.AddRange(items);
            ctx.SaveChanges();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
