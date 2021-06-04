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
                //await TestCase1.RunAsync(ctx);
                // await TestCase2.RunAsync(ctx);
                List<Book> books = new List<Book>();
                for(int i=0;i<100;i++)
                {
                    books.Add(new Book { AuthorName = "abc"+i, Price = new Random().NextDouble(), PubTime = DateTime.Now, Title = Guid.NewGuid().ToString() });
                }
                await ctx.BulkInsertAsync(books);
                // await ctx.BulkInsertAsync(new List<Comment>());
               // await ctx.BulkInsertAsync(new List<Article>());
            }
        }
    }
}
