using Demo.Base;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (TestDbContext ctx = new TestDbContext())
            {
                await ctx.BatchUpdate<Book>()
                    .Set("Title", "Haha")
                    .Set("Price", 3.14)
                    .Where(b => b.Price > 888)
                    .ExecuteAsync();
                await ctx.BatchUpdate<Book>()
                .Set("Title", "Haha")
                .Set("Price", 3)
                .Where(b => b.Price > 888)
                .ExecuteAsync();
                await TestCaseLimit.RunAsync(ctx);
            }
        }
    }
}
