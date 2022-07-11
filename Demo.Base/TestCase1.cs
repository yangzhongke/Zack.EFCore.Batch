using Microsoft.EntityFrameworkCore;

namespace Demo.Base
{
    public class TestCase1
    {
        public static async Task RunAsync(BaseDbContext ctx)
        {
            int n = Convert.ToInt32("3");
            string s = "hello";
            await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == s, true);
            await ctx.BatchUpdate<Book>()
                .Set(b => b.Price, b => b.Price + 3)
                .Set(b => b.Title, b => s)
                .Set(b => b.AuthorName, b => b.Title.Substring(3, 2) + b.AuthorName.ToUpper())
                .Set(b => b.PubTime, b => DateTime.Now)
                .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
                .ExecuteAsync();
            var b = await ctx.Books.OrderBy(b => b.PubTime).FirstOrDefaultAsync();
            Console.WriteLine(b);
            await ctx.BatchUpdate<Book>()
                .Where(x => x.AuthorName == s)
                .Set(x => x.Price, x => 10)
                .ExecuteAsync();
            await ctx.Books.DeleteRangeAsync(ctx, x => x.AuthorName == s);
        }
    }
}
