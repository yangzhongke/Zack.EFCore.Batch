using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (TestDbContext ctx = new TestDbContext())
            {
                
                int n = Convert.ToInt32("3");
                string s = "hello";
                //ctx.Database.BeginTransaction();
                long[] nums = new long[] { 3, 5, 6 };
                await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == s,true);
                await ctx.BatchUpdate<Book>()
                    .Set(b => b.Price, b => b.Price + 3)
                    .Set(b => b.Title, b => s)
                    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
                    .Set(b => b.PubTime, b => DateTime.Now)
                    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack")|| nums.Contains(b.Id))
                    .ExecuteAsync();

                var b = await ctx.Books.OrderBy(b => b.PubTime).FirstOrDefaultAsync();
                Console.WriteLine(b);
                await ctx.Books2.DeleteRangeAsync(ctx, b => b.AuthorName == "zack");
                await ctx.Books2.BatchUpdate(ctx)
                    .Set(b => b.Price, b => 3.0)
                    .Where(b => b.Id > 5)
                    .ExecuteAsync();
            }
        }
    }
}
