# Zack.EFCore.BatchInsert
从 .NET 7 开始，EF Core 已内置批量删除和批量更新支持，因此本库不再对 .NET 7 及以上版本提供批量删除/更新功能，[请查看官方文档](https://learn.microsoft.com/zh-cn/ef/core/what-is-new/ef-core-7.0/whatsnew?WT.mc_id=DT-MVP-5004444#executeupdate-and-executedelete-bulk-updates)。请使用微软官方的 `ExecuteDelete` / `ExecuteUpdate` API。  
本库在 .NET 7 及以上版本仍然支持**批量插入（BulkInsert）**。  
如需查看旧版批量删除/更新代码，请访问备份分支：[backup#before_batch_update_delete_being_removed](https://github.com/yangzhongke/Zack.EFCore.Batch/tree/backup%23before_batch_update_delete_being_removed)

使用本库，Entity Framework Core 用户可以快速批量插入数据。  
本库支持 Entity Framework Core 7/8 及以上版本。

## 为什么要开发这个功能？

Entity Framework Core 中可以通过 AddRange() 方法批量添加数据，但 AddRange() 添加的数据仍然是通过逐条 Insert 语句插入到数据库中的，执行效率比较低。我们知道可以通过 SqlBulkCopy 快速地批量插入大量数据到 SQLServer 数据库，因为 SqlBulkCopy 是把多条数据打成一个数据包发送到 SQLServer 的，所以插入效率非常高。MySQL、PostgreSQL、Oracle 也有类似的支持。

当然，直接使用 SqlBulkCopy 插入数据还需要开发者把数据填充到 DataTable，还需要处理列的映射等，还需要处理 ValueConverter 等问题，使用起来比较繁琐。因此我对这些能力进行封装，从而让 EF Core 的开发者能够以面向模型的方式来批量插入数据。

## 性能对比

经过测试在 SQLServer 数据库中插入 10 万条数据，用 AddRange() 方法耗时约 21 秒，而使用本库只需约 5 秒。

在 MySQL 中，使用 BulkInsert 需要在服务器和客户端都开启 local_infile：在 MySQL server 端执行 "local_infile=ON"，然后在连接字符串中添加 "AllowLoadLocalInfile=true"。

