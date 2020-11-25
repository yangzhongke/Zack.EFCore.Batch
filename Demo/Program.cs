using System;
using System.Collections.Generic;
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
                
                List<long> ids = new List<long> { 3, 5, 8 };
                //int i = await ctx.DeleteRangeAsync<Book>(b => b.Price > n || ids.Contains(b.Id) || b.AuthorName == s);
                //int i = await ctx.DeleteRangeAsync<Book>();
                await ctx.BatchUpdate<Book>().Set(b => b.Price, b => b.Price+3).Set(b => b.Title, b => s)
                    .Set(b=>b.PubTime,b=>DateTime.Now)
                .ExecuteAsync(b=>b.Id>n);
                //.ExecuteAsync();
            }
        }
    }
}
