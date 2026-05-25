# Zack.EFCore.BatchInsert
[中文文档 Chinese version](https://github.com/yangzhongke/Zack.EFCore.Batch/blob/main/README_CN.md)  

> **⚠️ Notice for users of old Zack.EFCore.Batch packages**  
> Since .NET 7, EF Core has built-in support for batch delete and update via [`ExecuteDelete` and `ExecuteUpdate`](https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates). Therefore, **this library no longer supports `DeleteRangeAsync` or `BatchUpdate`**. Please use Microsoft's official APIs instead.  
> Existing published NuGet packages are not affected.  
> To view the legacy code, see the backup branch: [backup#before_batch_update_delete_being_removed](https://github.com/yangzhongke/Zack.EFCore.Batch/tree/backup%23before_batch_update_delete_being_removed)

Using this library, Entity Framework Core users can **insert multiple records quickly** (BulkInsert).  
This library supports .NET 5, 6, 7, 8, 9, and 10. A single NuGet package covers all supported versions — no need to install different packages per .NET version.

> **Note on package naming:** Packages are named `Zack.EFCore.BatchInsert.*` (reflecting that only BulkInsert is supported). The legacy `Zack.EFCore.Batch.*` packages (for .NET 5/6) remain on NuGet but are no longer maintained.

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

## ⚠️ Important: Entity State After BulkInsert

`BulkInsert`/`BulkInsertAsync` bypasses EF Core's change tracker. It generates bulk-load SQL commands and executes them **directly against the database**, without going through EF Core's `SaveChanges` pipeline.

This has two important consequences:

**1. Database-generated values are not written back to entities.**  
Auto-increment primary keys, server-side defaults, and computed columns remain at their original in-memory values after the call. For example, if `Book.Id` is a database-generated integer, it will still be `0` on every entity object even after a successful insert.

**2. The DbContext cache is unaware of the inserted rows.**  
EF Core maintains an internal identity map (first-level cache). Because BulkInsert does not go through `SaveChanges`, the inserted rows are never registered in this cache. If you call `ctx.Books.Find(someId)` or run a LINQ query against the **same** DbContext immediately afterwards, EF Core may return stale results or miss the newly inserted rows entirely.

### Recommendation

After calling `BulkInsert`/`BulkInsertAsync`, choose one of the following patterns:

**Option A — Create a new DbContext (simplest):**
```csharp
using (var ctx = new TestDbContext())
{
    ctx.BulkInsert(books);
}
// Use a fresh DbContext for any subsequent queries
using (var ctx2 = new TestDbContext())
{
    var inserted = ctx2.Books.ToList();
}
```

**Option B — Re-query with the same DbContext:**
```csharp
using (var ctx = new TestDbContext())
{
    ctx.BulkInsert(books);
    // Re-query to get the up-to-date data including generated values
    var inserted = ctx.Books.AsNoTracking().ToList();
}
```

> **Note:** `AsNoTracking()` bypasses the identity cache so EF Core reads fresh data from the database instead of returning cached entities.



## Misc
The following databases have been tested: MS SQLServer, MySQL (Pomelo), PostgreSQL (Npgsql), Oracle.

## Integration tests in CI

Pull requests run real-database integration tests for SQL Server, MySQL, PostgreSQL, and Oracle with GitHub Actions.

The workflow file is `.github/workflows/pr-integration.yml`.

Connection strings are configured through environment variables:

- `TEST_DB_SQLSERVER_CS`
- `TEST_DB_MYSQL_CS`
- `TEST_DB_PG_CS`
- `TEST_DB_ORACLE_CS`

For unsupported databases, fallback behavior (`AddRange + SaveChanges`) is covered by tests in `Tests/Zack.EFCore.BatchInsert.Tests/BulkInsertExecutorRoutingTests.cs` using SQLite.

DM integration tests are not part of the PR workflow yet.

## Release to NuGet (maintainers)

NuGet publishing is automated by `.github/workflows/publish-nuget.yml`.

Release instructions (git tag trigger or GitHub Actions UI trigger) are documented in `NUGET_TAG_RELEASE.md`.

[Report of this library](https://www.reddit.com/r/dotnetcore/comments/k1esra/how_to_batch_delete_or_update_in_entity_framework/)  
