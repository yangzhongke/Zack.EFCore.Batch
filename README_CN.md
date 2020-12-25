# Zack.EFCore.Batch
使用这个开发包, Entity Framework Core 用户可以使用LINQ语句删除或者更新多条数据库记录，操作只执行一条SQL语句并且不需要首先把实体对象加载到内存中。 
这个开发包支持 Entity Framework Core 5.0以及更高版。  

操作说明:  
第一步，安装NuGet包:
Postgresql（使用Npgsql.EntityFrameworkCore.PostgreSQL）用户，请使用Install-Package Zack.EFCore.Batch.Npgsql

MS SQLServer用户，请使用Install-Package Zack.EFCore.Batch.MSSQL

MySQL（使用Pomelo.EntityFrameworkCore.MySql）用户，请使用Install-Package Zack.EFCore.Batch.MySQL.Pomelo

Sqlite用户，请使用Install-Package Zack.EFCore.Batch.Sqlite

第二步:
根据不同的数据库，请分别把如下代码添加到你的DbContext类的OnConfiguring方法中：
```csharp
optionsBuilder.UseBatchEF_MSSQL();// MSSQL Server 用户用这个
optionsBuilder.UseBatchEF_Npgsql();//Postgresql 用户用这个
optionsBuilder.UseBatchEF_MySQLPomelo();//MySQL 用户用这个
optionsBuilder.UseBatchEF_Sqlite();//Sqlite 用户用这个
```

第三步:
使用DbContext的扩展方法DeleteRangeAsync()来删除一批数据.
DeleteRangeAsync()的参数就是过滤条件的lambda表达式。
例子代码:
```csharp
await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == "zack yang"); 
```

上面的代码将会在数据库中执行如下SQL语句：
```SQL
Delete FROM [T_Books] WHERE ([Price] > @__p_0) OR ([AuthorName] = @__s_1)
```

DeleteRange()方法是DeleteRangeAsync()的同步方法版本。

使用DbContext的扩展方法BatchUpdate()来创建一个BatchUpdateBuilder对象。
BatchUpdateBuilder类有如下四个方法：
* Set()方法用于给一个属性赋值。方法的第一个参数是属性的lambda表达式,第二个参数是值的lambda表达式。
* Where() 是过滤条件
* ExecuteAsync()使用用于执行BatchUpdateBuilder的异步方法,Execute()是ExecuteAsync()的同步方法版本。

 例子代码:
```csharp
await ctx.BatchUpdate<Book>()
    .Set(b => b.Price, b => b.Price + 3)
    .Set(b => b.Title, b => s)
    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
    .Set(b => b.PubTime, b => DateTime.Now)
    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
    .ExecuteAsync();
```

上面的代码将会在数据库中执行如下SQL语句：
```SQL
Update [T_Books] SET [Price] = [Price] + 3.0E0, [Title] = @__s_1, [AuthorName] = COALESCE(SUBSTRING([Title], 3 + 1, 2), N'') + COALESCE(UPPER([AuthorName]), N''), [PubTime] = GETDATE()
WHERE ([Id] > @__p_0) OR ([AuthorName] IS NOT NULL AND ([AuthorName] LIKE N'Zack%'))
```

这个开发包使用EF Core实现的lambda表达式到SQL语句的翻译，所以几乎所有EF Core支持的lambda表达式写法都被支持。

以下数据库已经过测试，可以被Zack.EFCore.Batch支持: MS SQLServer(Microsoft.EntityFrameworkCore.SqlServer), MySQL(Pomelo.EntityFrameworkCore.MySql), PostgreSQL(Npgsql.EntityFrameworkCore.PostgreSQL)。

理论上来说，只要一个数据库有对应的EF Core 5的Provider，那么Zack.EFCore.Batch就可以支持这个数据库。如果您使用的数据库目前不在被支持的范围内，请提交Issue，我一般可以在一个工作日内开发完成。


[关于这个库的开发报告（B站）](https://www.bilibili.com/read/cv8545714)  

[关于这个库的开发报告（今日头条）](https://www.toutiao.com/i6899423396355293708/)  
