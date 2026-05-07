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
                // Sqlite has no native bulk-copy API; use EF Core's built-in AddRange + SaveChanges.
                List<Book> books = TestBulkInsert1.BuildBooks();
                ctx.AddRange(books);
                List<Author> authors = TestBulkInsert1.BuildAuthors();
                ctx.AddRange(authors);
                ctx.SaveChanges();
            }
        }
    }
}
