# Zack.EFCore.Batch
.NET 7开始，EF Core已经内置了对批量删除和批量更新的支持，因此本项目将不再.NET7中支持这两个功能（[详情点击这里](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates)）。但是本项目仍然在.NET 7中支持数据的批量插入。

使用这个开发包, Entity Framework Core 用户可以快速批量插入数据。
这个开发包支持 Entity Framework Core 7及以上版本。 

## 为什么开发这个功能？

Entity Framework Core中可以通过AddRange()方法来批量插入数据，但是AddRange()添加的数据仍然是被逐条执行Insert语句来插入到数据库中的，执行效率比较低。我们知道，我们可以通过SqlBulkCopy来快速地插入大量的数据到SQLServer数据库，因为SqlBulkCopy是把多条数据打成一个数据包发送到SQLServer的，所以插入效率非常高。MySQL、PostgreSQL等也有类似的支持。

当然，直接使用SqlBulkCopy来进行数据插入需要程序员把数据填充到DataTable，而且需要进行列的映射等操作，还需要处理ValueConverter等问题，用起来比较麻烦。因此我对这些功能封装，从而让EF Core的开发者能够更方便的以面向模型的方式来插入数据。

## 性能对比

我用SQLServer数据库做了一下插入10万条数据的测试，用AddRange插入耗时约21秒，而用我这个开源项目进行插入耗时只有约5秒。

## 操作说明:  

### 安装Nuget包：
```
SQLServer: Install-Package Zack.EFCore.Batch.MSSQL_NET7
MySQL: Install-Package Zack.EFCore.Batch.MySQL.Pomelo_NET7
Postgresql: Install-Package Zack.EFCore.Batch.Npgsql_NET7
```

### 批量插入数据
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
在 mysql中, 如果使用BulkInsert，请在服务器端和客户端都启用local_infile：在mysql server服务器端启用"local_infile=ON"，然后在连接字符串中添加 "AllowLoadLocalInfile=true"。
