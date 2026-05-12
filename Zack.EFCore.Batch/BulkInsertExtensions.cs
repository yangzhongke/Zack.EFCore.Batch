using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Logging;
using Zack.EFCore.Batch.Internal;

namespace System.Linq;

public static class BulkInsertExtensions
{
    public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
        IEnumerable<TEntity> items, CancellationToken cancellationToken = default) where TEntity : class
    {
        var entityList = items as IList<TEntity> ?? items.ToList();
        var executor = BulkInsertExecutorResolver.Resolve(dbCtx);
        if (executor == null)
        {
            LogFallback(dbCtx, typeof(TEntity));
            dbCtx.AddRange(entityList);
            await dbCtx.SaveChangesAsync(cancellationToken);
            MarkEntitiesUnchanged(dbCtx, entityList);
            return;
        }

        await executor.BulkInsertAsync(dbCtx, typeof(TEntity), entityList, cancellationToken);
        MarkEntitiesUnchanged(dbCtx, entityList);
    }

    public static void BulkInsert<TEntity>(this DbContext dbCtx,
        IEnumerable<TEntity> items) where TEntity : class
    {
        var entityList = items as IList<TEntity> ?? items.ToList();
        var executor = BulkInsertExecutorResolver.Resolve(dbCtx);
        if (executor == null)
        {
            LogFallback(dbCtx, typeof(TEntity));
            dbCtx.AddRange(entityList);
            dbCtx.SaveChanges();
            MarkEntitiesUnchanged(dbCtx, entityList);
            return;
        }

        executor.BulkInsert(dbCtx, typeof(TEntity), entityList);
        MarkEntitiesUnchanged(dbCtx, entityList);
    }

    private static void MarkEntitiesUnchanged<TEntity>(DbContext dbCtx, IEnumerable<TEntity> entities)
        where TEntity : class
    {
        foreach (var entity in entities)
        {
            dbCtx.Entry(entity).State = EntityState.Unchanged;
        }
    }

    private static void LogFallback(DbContext dbCtx, Type entityType)
    {
        try
        {
            var loggerFactory = dbCtx.GetService<ILoggerFactory>();
            var logger = loggerFactory.CreateLogger("Zack.EFCore.Batch.BulkInsert");
            logger.LogInformation(
                "No IBulkInsertExecutor matched provider {ProviderName}. Falling back to AddRange + SaveChanges for entity {EntityType}.",
                dbCtx.Database.ProviderName ?? "<unknown>",
                entityType.FullName ?? entityType.Name);
        }
        catch
        {
            // Logging failures should not block fallback insertion.
        }
    }
}