using Demo.Base;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
            using (TestDbContext ctx = new TestDbContext())
            {
                List<Book> books = TestBulkInsert1.BuildBooks();
                ctx.BulkInsert(books);
                List<Author> authors = TestBulkInsert1.BuildAuthors();
                ctx.BulkInsert(authors);

                var articles = TestOwnedType.BuildArticlesForInsert();
                ctx.BulkInsert(articles);
            }
        }
    }
}
