using System.Collections;
using Microsoft.EntityFrameworkCore;

namespace Zack.EFCore.Batch;

public interface IBulkInsertExecutor
{
    bool CanHandle(DbContext dbCtx);

    Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
        CancellationToken cancellationToken = default);

    void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items);
}