using Demo.Base;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using TestDbContext ctx = new TestDbContext();
            string title = null;
            await ctx.BatchUpdate<Book>()
                .Set(b => b.Title, b => title)
                .Where(b => b.Id <= 2)
                .ExecuteAsync();
            await ctx.BatchUpdate<Book>()
            .Set(a => a.Title, a => "测试")
            .Set(a => a.AuthorName, a => a.AuthorName)
            .Where(a => ctx.Articles.Where(b => b.Id == a.Id && b.Content == "B").Any())
            .ExecuteAsync();
            await TestCase1.RunAsync(ctx);
            await TestCase2.RunAsync(ctx);
            List<Book> books = TestBulkInsert1.BuildBooks();
            ctx.BulkInsert(books);
            List<Author> authors = TestBulkInsert1.BuildAuthors();
            ctx.BulkInsert(authors);
            await TestCaseLimit.RunAsync(ctx);
            await ctx.Comments.Where(c => c.Article.Id == 3).Take(10)
                .DeleteRangeAsync(ctx);
        }
    }
}
