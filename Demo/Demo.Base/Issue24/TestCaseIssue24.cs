namespace Demo.Base.Issue24;

public class TestCaseIssue24
{
    public static List<Book> BuildData()
    {
        var books = new List<Book>();
        for (var i = 0; i < 1000; i++)
            books.Add(new Book
            {
                AuthorName = "abc" + i, Price = new Random().NextDouble(), Title = Guid.NewGuid(), Id = Guid.NewGuid(),
                Is_Soft_Deleted = false, Available = false
            });
        return books;
    }
}