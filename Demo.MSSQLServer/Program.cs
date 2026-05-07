using Demo.Base;
using System.Diagnostics;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using TestDbContext ctx = new TestDbContext();

            List<Author> authors = TestBulkInsert1.BuildAuthors();
            ctx.BulkInsert(authors);
            ctx.BulkInsert(TestOwnedType.BuildArticlesForInsert());
            
            var items = TestOwnedType.BuildArticlesForInsert(100);
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            ctx.BulkInsert(items);
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
            stopwatch.Reset();
            stopwatch.Start();
            ctx.AddRange(items);
            ctx.SaveChanges();
            stopwatch.Stop();
            Console.WriteLine(stopwatch.Elapsed);
        }
    }
}
