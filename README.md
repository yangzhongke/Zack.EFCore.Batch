# Zack.EFCore.Batch
[中文文档 Chinese version](https://github.com/yangzhongke/Zack.EFCore.Batch/blob/main/README_CN.md)  

 Using this library, Entity Framework Core users can delete or update multiple records from a LINQ Query in a SQL statement without loading entities.
 This libary supports Entity Framework Core 5.0 and above.  

 Instructions:  
 Step 1:
```
 Install-Package Zack.EFCore.Batch
```

**Attention:** As for Postgresql (using Npgsql.EntityFrameworkCore.PostgreSQL) users, please use  Install-Package Zack.EFCore.Batch.Npgsql instead.

 Step 2:
 Add the following code into OnConfiguring() method of your DbContext
```csharp
 optionsBuilder.UseBatchEF();
```

**Attention:** As for Postgresql users, please use optionsBuilder.UseBatchEF_Npgsql() instead.

Step 3:
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
    .Set(b => b.PubTime, b => DateTime.Now)
    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
    .ExecuteAsync();
```
The code above will execute the following SQL statement on database：
```SQL
Update [T_Books] SET [Price] = [Price] + 3.0E0, [Title] = @__s_1, [AuthorName] = COALESCE(SUBSTRING([Title], 3 + 1, 2), N'') + COALESCE(UPPER([AuthorName]), N''), [PubTime] = GETDATE()
WHERE ([Id] > @__p_0) OR ([AuthorName] IS NOT NULL AND ([AuthorName] LIKE N'Zack%'))
```

This library utilizes the EF Core to translate the lambda expression to SQL statement, so it supports nearly all the lambda expressions which EF Core supports.

The following databases have been tested that they can work well with Zack.EFCore.Batch: MS SQLServer(Microsoft.EntityFrameworkCore.SqlServer), MySQL(Pomelo.EntityFrameworkCore.MySql), PostgreSQL(Npgsql.EntityFrameworkCore.PostgreSQL). 
In theory, as long as a database has its EF Core 5 Provider , the database can be supported by this library. In another words, if you are using a database that does not already have an EF Core 5 Provider, the library will not support it either.

[Report of this library](https://www.reddit.com/r/dotnetcore/comments/k1esra/how_to_batch_delete_or_update_in_entity_framework/)  
