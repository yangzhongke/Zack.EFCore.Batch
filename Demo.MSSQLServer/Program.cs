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
            /*
            using (TestDbContext ctx = new TestDbContext())
            {
                //await TestCase1.RunAsync(ctx);
                // await TestCase2.RunAsync(ctx);
                List<Book> books = TestBulkInsert1.BuildBooks();
                ctx.BulkInsert(books);
               // await ctx.BulkInsertAsync(books);
                // await ctx.BulkInsertAsync(new List<Comment>());
               // await ctx.BulkInsertAsync(new List<Article>());
            }*/
            using (TestDbContext ctx = new TestDbContext())
            {
                await TestCaseLimit.RunAsync(ctx);
            }                
        }
    }
}
