using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base
{
    public class TestBulkInsert1
    {
        public static List<Book> BuildBooks()
        {
            List<Book> books = new List<Book>();
            for (int i = 0; i < 100; i++)
            {
                books.Add(new Book { AuthorName = "abc" + i, Price = new Random().NextDouble(), PubTime = DateTime.Now, Title = Guid.NewGuid().ToString() });
            }
            books[0].Pages = 3;
            return books;
        }
    }
}
