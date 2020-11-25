# Zack.EFCore.Batch
使用这个开发包, Entity Framework Core 用户可以使用LINQ语句删除或者更新多条数据库记录，操作只执行一条SQL语句并且不需要首先把实体对象加载到内存中。 
这个开发包支持 Entity Framework Core 5.0以及更高版。  

操作说明:  
第一步:
```
 Install-Package Zack.EFCore.Batch
```
第二步:
把如下代码添加到你的DbContext类的OnConfiguring方法中：
```
 optionsBuilder.UserBatchEF();
```
第三步:
使用DbContext的扩展方法DeleteRangeAsync()来删除一批数据.
DeleteRangeAsync()的参数就是过滤条件的lambda表达式。
例子代码:
```
await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == "zack yang"); 
```
DeleteRange()方法是DeleteRangeAsync()的同步方法版本。

使用DbContext的扩展方法BatchUpdate()来创建一个BatchUpdateBuilder对象。
BatchUpdateBuilder类有如下四个方法：
* Set()方法用于给一个属性赋值。方法的第一个参数是属性的lambda表达式,第二个参数是值的lambda表达式。
* Where() 是过滤条件
* ExecuteAsync()使用用于执行BatchUpdateBuilder的异步方法,Execute()是ExecuteAsync()的同步方法版本。

 例子代码:
 ```
await ctx.BatchUpdate<Book>()
    .Set(b => b.Price, b => b.Price + 3)
    .Set(b => b.Title, b => s)
    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
    .Set(b => b.PubTime, b => DateTime.Now)
    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
    .ExecuteAsync();
```