using System.Diagnostics;
using Demo.Base;

namespace Demo;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var ctx = new TestDbContext();

        var authors = TestBulkInsert1.BuildAuthors();
        ctx.BulkInsert(authors);
        ctx.BulkInsert(TestOwnedType.BuildArticlesForInsert());

        var items = TestOwnedType.BuildArticlesForInsert(100);
        var stopwatch = new Stopwatch();
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