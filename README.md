# Zack.EFCore.Batch
[中文文档 Chinese version](https://github.com/yangzhongke/Zack.EFCore.Batch/blob/main/README_CN.md)  

 Using this library, Entity Framework Core users can delete or update multiple records from a LINQ Query in a SQL statement without loading entities.
 This libary supports Entity Framework Core 5.0 and Entity Framework Core 6.0.  

 ## Instructions:  
 
 ##### Step 1 
As for.NET 5 users:
```
SQLServer: Install-Package Zack.EFCore.Batch.MSSQL
MySQL: Install-Package Zack.EFCore.Batch.MySQL.Pomelo
Postgresql: Install-Package Zack.EFCore.Batch.Npgsql
Sqlite: Install-Package Zack.EFCore.Batch.Sqlite
Oracle:Install-Package Zack.EFCore.Batch.Oracle
``` 
As For.NET 6 users:
```
SQLServer: Install-Package Zack.EFCore.Batch.MSSQL_NET6
MySQL: Install-Package Zack.EFCore.Batch.MySQL.Pomelo_NET6
Postgresql: Install-Package Zack.EFCore.Batch.Npgsql_NET6
Sqlite: Install-Package Zack.EFCore.Batch.Sqlite_NET6
Oracle: Install-Package Zack.EFCore.Batch.Oracle_NET6
```
Support of MySQL is based on Pomelo.EntityFrameworkCore.MySql.

 ##### Step 2:
 Depending on the database, add the following code into OnConfiguring() method of your DbContext respectively.
```csharp
optionsBuilder.UseBatchEF_MSSQL();// as for MSSQL Server
optionsBuilder.UseBatchEF_Npgsql();//as for Postgresql
optionsBuilder.UseBatchEF_MySQLPomelo();//as for MySQL
optionsBuilder.UseBatchEF_Sqlite();//as for Sqlite
optionsBuilder.UseBatchEF_Oracle();//as for Oracle

```

##### Step 3:
Use the extension method DeleteRangeAsync() of DbContext to delete a set of records.
The parameter of DeleteRangeAsync() is the lambda expression of the filter
Example code:
```csharp
await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == "zack yang"); 
```

The code above will execute the following SQL statement on database：
```SQL
Delete FROM [T_Books] WHERE ([Price] > @__p_0) OR ([AuthorName] = @__s_1)
```

and the DeleteRange() is the synchronous version of DeleteRangeAsync().

Use the extension method BatchUpdate() of DbContext to create a BatchUpdateBuilder.
There are four methods in BatchUpdateBuilder as follows
* Set() is used for assigning a value to a property. The first parameter of the method is the lambda expression of the property, and the second one is the lambda expression of the value.
* Where() is used for setting the filter expression
* ExecuteAsync() is an asynchronous method that can execute the BatchUpdateBuilder, and the Execute() is a synchronous alternative of ExecuteAsync()

 Example code:
```csharp
await ctx.BatchUpdate<Book>()
    .Set(b => b.Price, b => b.Price + 3)
    .Set(b => b.Title, b => s)
    .Set(b => b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
    .Set(b => b.PubTime, DateTime.Now)
    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
    .ExecuteAsync();
```
The code above will execute the following SQL statement on database：
```SQL
Update [T_Books] SET [Price] = [Price] + 3.0E0, [Title] = @__s_1, [AuthorName] = COALESCE(SUBSTRING([Title], 3 + 1, 2), N'') + COALESCE(UPPER([AuthorName]), N''), [PubTime] = GETDATE()
WHERE ([Id] > @__p_0) OR ([AuthorName] IS NOT NULL AND ([AuthorName] LIKE N'Zack%'))
```

## Take(), Skip()
Take() and Skip() can be used to limit the affected rows of DeleteRangeAsync and BatchUpdate:
```CSharp
await ctx.Comments.Where(c => c.Article.Id == id).OrderBy(c => c.Message)
.Skip(3).DeleteRangeAsync<Comment>(ctx);
await ctx.Comments.Where(c => c.Article.Id == id).Skip(3).Take(10).DeleteRangeAsync<Comment>(ctx);
await ctx.Comments.Where(c => c.Article.Id == id).Take(10).DeleteRangeAsync<Comment>(ctx);

await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
	.Where(c => c.Article.Id == id)
	.Skip(3)
	.ExecuteAsync();

await ctx.BatchUpdate<Comment>()
	.Set(c => c.Message, c => "abc")
	.Where(c => c.Article.Id == id)
	.ExecuteAsync();
	
await ctx.BatchUpdate<Comment>()
	.Set("Message","abc")
	.Where(c => c.Article.Id == id)
	.ExecuteAsync();

await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
	.Where(c => c.Article.Id == id)
	.Skip(3)
	.Take(10)
	.ExecuteAsync();
await ctx.BatchUpdate<Comment>().Set(c => c.Message, c => c.Message + "abc")
   .Where(c => c.Article.Id == id)
   .Take(10)
   .ExecuteAsync();
```

## BulkInsert

At this point, BulkInsert cannot be supported on SqlLite.
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


## Misc
This library utilizes the EF Core to translate the lambda expression to SQL statement, so it supports nearly all the lambda expressions which EF Core supports.

The following databases have been tested that they can work well with Zack.EFCore.Batch: MS SQLServer(Microsoft.EntityFrameworkCore.SqlServer), MySQL(Pomelo.EntityFrameworkCore.MySql), PostgreSQL(Npgsql.EntityFrameworkCore.PostgreSQL), Oracle(Oracle.EntityFrameworkCore). 
In theory, as long as a database has its EF Core 5/6 Provider , the database can be supported by this library. If you are using a database that is not currently supported, please submit an issue. I can usually complete the development within one working day.

[Report of this library](https://www.reddit.com/r/dotnetcore/comments/k1esra/how_to_batch_delete_or_update_in_entity_framework/)  
