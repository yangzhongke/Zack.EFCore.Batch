using System;
using System.Linq;
using System.Threading.Tasks;

namespace Demo.Base
{
    public class TestCase2
    {
        public static async Task RunAsync(BaseDbContext ctx)
        {
            int id = Convert.ToInt32("3");
            await ctx.DeleteRangeAsync<Comment>(c => c.Article.Id == id);
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc" )
                .Where(c => c.Article.Id == id)
                .ExecuteAsync();
        }
    }
}
