# Zack.EFCore.Batch
.NET 7开始，EF Core已经内置了对批量删除和批量更新的支持，因此本项目将不再.NET7中支持这两个功能（[详情点击这里](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates)）。但是本项目仍然在.NET 7中支持数据的批量插入。

使用这个开发包, Entity Framework Core 用户可以快速批量插入数据。
这个开发包支持 Entity Framework Core 7及以上版本。  

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
