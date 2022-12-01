# Zack.EFCore.Batch

 Using this library, Entity Framework Core users can insert multiple records quickly.
 This libary supports Entity Framework Core 7 and above.  

## Instructions:  
 
### Install Nuget package:

```
SQLServer: Install-Package Zack.EFCore.Batch.MSSQL_NET7
MySQL: Install-Package Zack.EFCore.Batch.MySQL.Pomelo_NET7
Postgresql: Install-Package Zack.EFCore.Batch.Npgsql_NET7
``` 

### BulkInsert

```
List<Book> books = new List<Book>();
for (int i = 0; i < 100; i++)
{
	books.Add(new Book { AuthorName = "abc" + i, Price = new Random().NextDouble(), PubTime = DateTime.Now, Title = Guid.NewGuid().ToString() });
}
using (TestDbContext ctx = new TestDbContext())
{
	ctx.BulkInsert(books);
}
```
On mysql, to use BulkInsert, please enable local_infile on server side and client side: enable "local_infile=ON" on mysql server, and add "AllowLoadLocalInfile=true" to connection string on client side.