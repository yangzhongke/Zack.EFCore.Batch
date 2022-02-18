using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base
{
    public class TestCaseLimit
    {
        public static async Task RunAsync(BaseDbContext ctx)
        {
            int id = Convert.ToInt32("3");

            await ctx.DeleteRangeAsync<Comment>(c => c.Article.Id == id);
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
                .Where(c => c.Article.Id == id)
                .ExecuteAsync();

            await ctx.Comments.Where(c => c.Article.Id == id).DeleteRangeAsync<Comment>(ctx);
            await ctx.Comments.Where(c => c.Article.Id == id).Skip(3).DeleteRangeAsync<Comment>(ctx);
            await ctx.Comments.Where(c => c.Article.Id == id).Skip(3).Take(10).DeleteRangeAsync<Comment>(ctx);
            await ctx.Comments.Where(c => c.Article.Id == id).Take(10).DeleteRangeAsync<Comment>(ctx);
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
                .Where(c => c.Article.Id == id)
                .Skip(3)
                .ExecuteAsync();

            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
                .Where(c => c.Article.Id == id)
                .Skip(3)
                .Take(10)
                .ExecuteAsync();
            await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
               .Where(c => c.Article.Id == id)
               .Take(10)
               .ExecuteAsync();
        }
    }
}
