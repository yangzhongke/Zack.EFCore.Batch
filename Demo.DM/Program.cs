using Demo.Base;

namespace Demo;

internal class Program
{
    private static async Task Main(string[] args)
    {
        using var ctx = new TestDbContext();
        var books = TestBulkInsert1.BuildBooks();
        ctx.BulkInsert(books);
        var authors = TestBulkInsert1.BuildAuthors();
        ctx.BulkInsert(authors);
    }
}