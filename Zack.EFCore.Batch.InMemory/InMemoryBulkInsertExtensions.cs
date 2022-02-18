using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.InMemory;

namespace System.Linq
{
    public static class InMemoryBulkInsertExtensions
    {
        public static Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default) where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(dbCtx);
            dbCtx.AddRange(items);
            return dbCtx.SaveChangesAsync(cancellationToken);
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items) where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(dbCtx);
            dbCtx.AddRange(items);
            dbCtx.SaveChanges();
        }
    }
}
