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
                    Price = new Random().Next(), 
                    PubTime = DateTime.Now, 
                    Title = Guid.NewGuid().ToString() };
				b.BookType = i % 2 == 0?BookType.Fictional: BookType.Scientific;
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
                if (i < 10)
                {
                    b.Tags = new List<string>();
                    b.Tags.Add("xxx");
                    b.Tags.Add("yyy");
                }
                authors.Add(b);
            }
            return authors;
        }
    }
}
