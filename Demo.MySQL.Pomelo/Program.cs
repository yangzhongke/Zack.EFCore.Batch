using Demo.Base;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (TestDbContext ctx = new TestDbContext())
            {
                //await TestCase1.RunAsync(ctx);
                List<Book> books = TestBulkInsert1.BuildBooks();
                ctx.BulkInsert(books);
            }
        }
    }
}
