# Zack.EFCore.Batch
[中文文档 Chinese version](https://github.com/yangzhongke/Zack.EFCore.Batch/blob/main/README_CN.md)  

> **⚠️ Notice for .NET 7+ users**  
> Since .NET 7, EF Core has built-in support for batch delete and update via [`ExecuteDelete` and `ExecuteUpdate`](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates). Therefore, **this library no longer supports `DeleteRangeAsync` or `BatchUpdate`**. Please use Microsoft's official APIs instead.  
> Existing published NuGet packages for .NET 5/6 are not affected.  
> To view the legacy code, see the backup branch: [backup#before_batch_update_delete_being_removed](https://github.com/yangzhongke/Zack.EFCore.Batch/tree/backup%23before_batch_update_delete_being_removed)

Using this library, Entity Framework Core users can **insert multiple records quickly** (BulkInsert).  
This library supports .NET 5, 6, 7, 8, 9, and 10. A single NuGet package covers all supported versions — no need to install different packages per .NET version.

> **Note on package naming:** The new packages are named `Zack.EFCore.BatchInsert.*` (reflecting that only BulkInsert is supported going forward). They do not conflict with the legacy `Zack.EFCore.Batch.*` packages.

## Instructions:  
 
##### Step 1 
Install the package for your database. One package supports .NET 5 through 8:
```
SQLServer:  Install-Package Zack.EFCore.BatchInsert.MSSQL
MySQL:      Install-Package Zack.EFCore.BatchInsert.MySQL.Pomelo
Postgresql: Install-Package Zack.EFCore.BatchInsert.Npgsql
Oracle:     Install-Package Zack.EFCore.BatchInsert.Oracle
Dm(达梦):   Install-Package Zack.EFCore.BatchInsert.Dm
```

Support of MySQL is based on Pomelo.EntityFrameworkCore.MySql.

##### Step 2:
No extra configuration is required anymore. After installing the provider package, you can call `BulkInsert`/`BulkInsertAsync` directly.

## BulkInsert

```csharp
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
On MySQL, to use BulkInsert, please enable `local_infile` on both the server and client side: set `local_infile=ON` on the MySQL server, and add `AllowLoadLocalInfile=true` to the connection string on the client side.



## Misc
This library utilizes EF Core to translate lambda expressions to SQL statements, so it supports nearly all lambda expressions which EF Core supports.

The following databases have been tested: MS SQLServer, MySQL (Pomelo), PostgreSQL (Npgsql), Oracle.

## Integration tests in CI

Pull requests run real-database integration tests for SQL Server, MySQL, PostgreSQL, and Oracle with GitHub Actions.

The workflow file is `.github/workflows/pr-integration.yml`.

Connection strings are configured through environment variables:

- `TEST_DB_SQLSERVER_CS`
- `TEST_DB_MYSQL_CS`
- `TEST_DB_PG_CS`
- `TEST_DB_ORACLE_CS`

For unsupported databases, fallback behavior (`AddRange + SaveChanges`) is covered by tests in `Tests/Zack.EFCore.Batch.Tests/BulkInsertExecutorRoutingTests.cs` using SQLite.

DM integration tests are not part of the PR workflow yet.

[Report of this library](https://www.reddit.com/r/dotnetcore/comments/k1esra/how_to_batch_delete_or_update_in_entity_framework/)  
