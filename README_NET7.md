# Zack.EFCore.Batch
Since EFCore 7 has built-in support of 'batch update and delete' ([please see this page](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates)), this library doesn't support EFCore 7 and higher anymore), but BulkInsert is still supported.

 Using this library, Entity Framework Core users can insert multiple records quickly.
 This libary supports Entity Framework Core 7 and above.  

## Why did I develop this feature?

The AddRange() method can be used to batch insert data in Entity Framework Core. However, the data added by AddRange() is still inserted into the database by using INSERT statements one by one, which is inefficient. We know that SqlBulkCopy can quickly insert a large amount of data to SQLServer database, because SqlBulkCopy can pack multiple data into a packet and send it to SQLServer, so the insertion efficiency is very high. MySQL, PostgreSQL, and others have similar support.

Of course, using SqlBulkCopy to insert data directly requires the programmer to fill the data to the DataTable, perform column mapping, and handle ValueConverter and other issues, which is troublesome to use. Therefore, I encapsulated these capabilities to make it easier for EF Core developers to insert data in a model-oriented manner.

This library currently supports MS SQLServer, MySQL, and PostgreSQL databases.

## Comparison of performance

I did a test of inserting 100,000 pieces of data with SQLServer database, and the insertion took about 21 seconds with AddRange(), compared to only about 5 seconds with my open-source project.

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