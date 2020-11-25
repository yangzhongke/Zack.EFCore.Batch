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
                /*
                List<long> ids = new List<long> { 3, 5, 8 };
                int i = await ctx.DeleteRangeAsync<Book>(b => b.Price > n || ids.Contains(b.Id) || b.AuthorName == s);
                //int i = await ctx.DeleteRangeAsync<Book>();
                await ctx.BatchUpdate<Book>().Set(b => b.Price, b => b.Price+3).Set(b => b.Title, b => s)
                    .Set(b=>b.PubTime,b=>DateTime.Now)
                .ExecuteAsync(b=>b.Id>n);
                //.ExecuteAsync();*/
                
                await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == s);
                await ctx.BatchUpdate<Book>()
                    .Set(b => b.Price, b => b.Price + 3)
                    .Set(b => b.Title, b => s)
                    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
                    .Set(b => b.PubTime, b => DateTime.Now)
                    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
                    .ExecuteAsync();
                
                /*
                string sss = ctx.Books.Where(b=>b.Id>2).Select(b => new { a=b.Title.Substring(3),w=b.Price+3 }).ToQueryString();
                Console.WriteLine(sss);*/
            }
        }
    }
}
