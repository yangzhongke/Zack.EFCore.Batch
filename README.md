# Zack.EFCore.Batch
[ÖÐÎÄÎÄµµ Chinese version](/Zack.EFCore.Batch/blob/main/README_CN.md)  

 Using this library, Entity Framework Core users can delete or update multiple records from a LINQ Query in a SQL statement without loading entities.
 This libary supports Entity Framework Core 5.0 and above.  

 Instructions:  
 Step 1:
```
 Install-Package Zack.EFCore.Batch
```
 Step 2:
 Add the following code to OnConfiguring of your DbContext
```
 optionsBuilder.UserBatchEF();
```
Step 3:
Using the extension method DeleteRangeAsync() of DbContext to delete a set of records.
The parameter of DeleteRangeAsync() is the lambda expression of the filter
 Example code:
```
await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == "zack yang"); 
```
and the DeleteRange() is the synchronous version of DeleteRangeAsync().

Using the extension method BatchUpdate() of DbContext to create a BatchUpdateBuilder.
There are four methods in BatchUpdateBuilder as follows
* Set() is used for assigning a value to a property. The first parameter of the method is the lambda expression of the property, and the second one is the lambda expression of the value.
* Where() is used for setting the filter expression
* ExecuteAsync() is an asynchronous method that can execute the BatchUpdateBuilder, and the Execute() is a synchronous alternative of ExecuteAsync()

 Example code:
 ```
await ctx.BatchUpdate<Book>()
    .Set(b => b.Price, b => b.Price + 3)
    .Set(b => b.Title, b => s)
    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2)+b.AuthorName.ToUpper())
    .Set(b => b.PubTime, b => DateTime.Now)
    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
    .ExecuteAsync();
```