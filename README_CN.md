# Zack.EFCore.BatchInsert
[English version](https://github.com/yangzhongke/Zack.EFCore.Batch/blob/main/README.md)

> **⚠️ 使用旧版Zack.EFCore.Batch包的用户请注意**  
> 从 .NET 7 开始，EF Core 已内置批量删除和批量更新支持（[`ExecuteDelete` 和 `ExecuteUpdate`](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates)）。因此，**本库已移除对 `DeleteRangeAsync` 和 `BatchUpdate` 的支持**，请使用微软官方 API。  
> 已发布的NuGet 包不受影响。  
> 如需查看旧版代码，请访问备份分支：[backup#before_batch_update_delete_being_removed](https://github.com/yangzhongke/Zack.EFCore.Batch/tree/backup%23before_batch_update_delete_being_removed)

使用本库，Entity Framework Core 用户可以快速**批量插入**数据（BulkInsert）。  
本库支持 .NET 5、6、7、8、9 和 10，单个 NuGet 包覆盖所有受支持的版本，无需为不同 .NET 版本安装不同的包。

> **关于包名：** 包命名为 `Zack.EFCore.BatchInsert.*`（体现本库仅支持批量插入）。旧版 `Zack.EFCore.Batch.*` 包（适用于 .NET 5/6）仍在 NuGet 上，但不再维护。

## 安装说明：

##### 第一步
安装对应数据库的包，一个包支持 .NET 5 至 10：
```
SQLServer:  Install-Package Zack.EFCore.BatchInsert.MSSQL
MySQL:      Install-Package Zack.EFCore.BatchInsert.MySQL.Pomelo
Postgresql: Install-Package Zack.EFCore.BatchInsert.Npgsql
Oracle:     Install-Package Zack.EFCore.BatchInsert.Oracle
Dm(达梦):   Install-Package Zack.EFCore.BatchInsert.Dm
```

MySQL 支持基于 Pomelo.EntityFrameworkCore.MySql。

##### 第二步：
现在不再需要额外配置。安装对应数据库包后，可直接调用 `BulkInsert`/`BulkInsertAsync`。

## BulkInsert（批量插入）

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
在 MySQL 中，使用 BulkInsert 需要在服务器和客户端都开启 `local_infile`：在 MySQL 服务器端设置 `local_infile=ON`，并在客户端连接字符串中添加 `AllowLoadLocalInfile=true`。

## ⚠️ 重要说明：BulkInsert 后的实体状态问题

`BulkInsert`/`BulkInsertAsync` 绕过了 EF Core 的变更追踪器（Change Tracker）。它直接向数据库执行批量写入命令，**不经过** EF Core 的 `SaveChanges` 流程。

这会带来两个重要影响：

**1. 数据库生成的值不会写回实体对象。**  
自增主键、服务器端默认值、计算列等，在调用完成后，在内存中的实体对象上仍保持原始值。例如，如果 `Book.Id` 是数据库自动生成的整型主键，即使插入成功，每个实体对象上的 `Id` 仍然是 `0`。

**2. DbContext 缓存对插入的记录一无所知。**  
EF Core 内部维护一个身份映射缓存（Identity Map，即一级缓存）。由于 BulkInsert 不走 `SaveChanges`，插入的记录不会被注册到这个缓存中。如果在同一个 DbContext 上立即调用 `ctx.Books.Find(someId)` 或执行 LINQ 查询，EF Core 可能返回过时的缓存结果，甚至查不到刚刚插入的记录。

### 建议

调用 `BulkInsert`/`BulkInsertAsync` 后，请采用以下方式之一：

**方案 A——创建新的 DbContext（最简单）：**
```csharp
using (var ctx = new TestDbContext())
{
    ctx.BulkInsert(books);
}
// 后续操作使用全新的 DbContext 实例
using (var ctx2 = new TestDbContext())
{
    var inserted = ctx2.Books.ToList();
}
```

**方案 B——用同一个 DbContext 重新查询：**
```csharp
using (var ctx = new TestDbContext())
{
    ctx.BulkInsert(books);
    // 重新查询以获取包含数据库生成值的最新数据
    var inserted = ctx.Books.AsNoTracking().ToList();
}
```

> **说明：** `AsNoTracking()` 会绕过身份缓存，让 EF Core 直接从数据库读取最新数据，而不是返回缓存中的旧实体。



## 补充说明
已测试可正常使用的数据库：MS SQLServer、MySQL (Pomelo)、PostgreSQL (Npgsql)、Oracle。

## CI 集成测试

针对 Pull Request，GitHub Actions 会连接真实数据库运行集成测试，当前覆盖 SQL Server、MySQL、PostgreSQL、Oracle。

工作流文件：`.github/workflows/pr-integration.yml`。

连接字符串通过以下环境变量配置：

- `TEST_DB_SQLSERVER_CS`
- `TEST_DB_MYSQL_CS`
- `TEST_DB_PG_CS`
- `TEST_DB_ORACLE_CS`

对于不支持的数据库，保底行为（`AddRange + SaveChanges`）通过 `Tests/Zack.EFCore.BatchInsert.Tests/BulkInsertExecutorRoutingTests.cs` 中的 SQLite 测试覆盖。

DM 的集成测试暂未纳入 PR 自动流程。

## 发布到 NuGet（维护者）

NuGet 发布由 `.github/workflows/publish-nuget.yml` 自动完成。

发布说明（git tag 触发或 GitHub Actions 界面触发）详见 `NUGET_TAG_RELEASE.md`。
