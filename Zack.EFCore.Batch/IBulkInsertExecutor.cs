using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;

namespace Zack.EFCore.Batch
{
    public interface IBulkInsertExecutor
    {
        bool CanHandle(DbContext dbCtx);

        Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items, CancellationToken cancellationToken = default);

        void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items);
    }
}

