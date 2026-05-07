using Demo.Base;
using Demo.DM_NET6;
using Dm;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using TestDbContext ctx = new TestDbContext();
            List<Book> books = TestBulkInsert1.BuildBooks();
            ctx.BulkInsert(books);
            List<Author> authors = TestBulkInsert1.BuildAuthors();
            ctx.BulkInsert(authors);
        }
    }
}
