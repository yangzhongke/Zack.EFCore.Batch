using System;
using System.Collections.Generic;

namespace Demo.Base
{
    public class TestBulkInsert1
    {
        public static List<Book> BuildBooks()
        {
            List<Book> books = new List<Book>();
            for (int i = 0; i < 100; i++)
            {
                Book b = new Book { AuthorName = "abc" + i, 
                    Price = new Random().NextDouble(), 
                    PubTime = DateTime.Now, 
                    Title = Guid.NewGuid().ToString() };
                books.Add(b);
            }
            books[0].Pages = 3;
            return books;
        }
        public static List<Author> BuildAuthors()
        {
            List<Author> authors = new List<Author>();
            for (int i = 0; i < 100; i++)
            {
                Author b = new Author {Name="hello"+i };
                authors.Add(b);
            }
            return authors;
        }
    }
}
