using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.Internal;

namespace System.Linq
{
    public static class MSSQLBulkInsertExtensions
    {
        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, SqlTransaction externalTransaction = null, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx,dbCtx.Set<TEntity>(), items);
            using (SqlBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((SqlConnection)conn, dbCtx,externalTransaction,copyOptions))
            {                
                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            }
        }

        private static SqlBulkCopy BuildSqlBulkCopy<TEntity>(SqlConnection conn,DbContext dbCtx,SqlTransaction externalTransaction, SqlBulkCopyOptions copyOptions) where TEntity : class
        {
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn,copyOptions,externalTransaction);
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = BulkInsertUtils.ParseDbProps<TEntity>(dbCtx,entityType);
            bulkCopy.DestinationTableName = entityType.GetSchemaQualifiedTableName();//Schema may be used
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName, columnName);
            }
            return bulkCopy;
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, SqlTransaction externalTransaction = null, SqlBulkCopyOptions copyOptions = SqlBulkCopyOptions.Default) where TEntity : class
        {            
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, dbCtx.Set<TEntity>(), items);
            using (SqlBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((SqlConnection)conn, dbCtx,externalTransaction,copyOptions))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
