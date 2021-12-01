using Demo.Base;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            
            using (TestDbContext ctx = new TestDbContext())
            {
                
                string title = null;
                await ctx.BatchUpdate<Book>()
                    .Set(b => b.Title, b => title)
                    .Where(b=>b.Id<=2)
                    .ExecuteAsync();
                await ctx.BatchUpdate<Book>()
                .Set(a => a.Title, a => "测试")
                .Where(a => ctx.Articles.Where(b => b.Id == a.Id && b.Content == "B").Any())
                .ExecuteAsync();
                await TestCase1.RunAsync(ctx);
                await TestCase2.RunAsync(ctx);
                //List<Book> books = TestBulkInsert1.BuildBooks();
                //ctx.BulkInsert(books);
                // await ctx.BulkInsertAsync(books);
                // await ctx.BulkInsertAsync(new List<Comment>());
                // await ctx.BulkInsertAsync(new List<Article>());
            }
            /*
            using (TestDbContext ctx = new TestDbContext())
            {
                //await TestCaseLimit.RunAsync(ctx);
                //List<Book> books = TestBulkInsert1.BuildBooks();
                //ctx.BulkInsert(books);
                await ctx.Comments.Where(c => c.Article.Id == 3).Take(10).DeleteRangeAsync<Comment>(ctx);
            }*/
        }
    }
}
