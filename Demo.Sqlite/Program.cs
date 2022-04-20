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
                await TestCaseLimit.RunAsync(ctx);
                await ctx.BatchUpdate<Book>()
                .Set("Title",null)
                .ExecuteAsync();
            }
        }
    }
}
