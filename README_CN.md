# Zack.EFCore.Batch
 Using this library, Entity Framework Core users can delete or update multiple records from a LINQ Query in a SQL statement without loading entities.
 ```
 Install-Package Zack.EFCore.Batch
 ```
 Instructions:  
 Step 1:
 Add the following code to OnConfiguring of your DbContext
  ```
 optionsBuilder.UserBatchEF();
  ```
Step2:
 Example code:
  ```
 
  ```
