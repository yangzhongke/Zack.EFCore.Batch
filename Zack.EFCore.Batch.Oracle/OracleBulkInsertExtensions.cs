using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.Oracle
{
    public static class OracleBulkInsertExtensions
    {
        private static OracleBulkCopy BuildSqlBulkCopy<TEntity>(OracleConnection conn, DbContext dbCtx, OracleBulkCopyOptions copyOptions) where TEntity : class
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = BulkInsertUtils.ParseDbProps<TEntity>(dbCtx, entityType);

            OracleBulkCopy bulkCopy = new OracleBulkCopy(conn, copyOptions);

            bulkCopy.DestinationTableName = entityType.GetTableName();
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName,columnName);
            }
            return bulkCopy;
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, OracleBulkCopyOptions copyOptions= OracleBulkCopyOptions.Default, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, dbCtx.Set<TEntity>(), items);
            using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx, copyOptions))
            {
                bulkCopy.WriteToServer(dataTable);
            }            
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items,  OracleBulkCopyOptions copyOptions = OracleBulkCopyOptions.Default, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, dbCtx.Set<TEntity>(), items);
            using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx,copyOptions))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
