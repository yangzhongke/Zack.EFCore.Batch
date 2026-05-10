namespace Demo.Base;

public class TestCaseLimit
{
    public static Task RunAsync(BaseDbContext ctx)
    {
        // DeleteRangeAsync and BatchUpdate have been removed from this library.
        // .NET 7+ users: please use EF Core's built-in ExecuteDelete / ExecuteUpdate instead.
        // See: https://learn.microsoft.com/en-us/ef/core/what-is-new/ef-core-7.0/whatsnew#executeupdate-and-executedelete-bulk-updates
        // For legacy code, see the backup branch: backup#before_batch_update_delete_being_removed
        return Task.CompletedTask;
    }
}