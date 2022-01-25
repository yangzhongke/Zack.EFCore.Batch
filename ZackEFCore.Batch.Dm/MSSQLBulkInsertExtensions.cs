using Dm;
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
            IEnumerable<TEntity> items, DmTransaction externalTransaction = null, DmBulkCopyOptions copyOptions = DmBulkCopyOptions.Default, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            using (DmBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((DmConnection)conn, dbCtx,externalTransaction,copyOptions))
            {
                await Task.Run(() => {
                    WriteToServer(bulkCopy,dataTable);
                },cancellationToken);
            }
        }

        private static void WriteToServer(DmBulkCopy bulkCopy,DataTable dataTable)
        {
            try
            {
                bulkCopy.WriteToServer(dataTable);
            }
            catch(DmException ex)
            {
                if(ex.Message.Contains("fastloading dll not loading"))
                {
                    throw new Exception($"{ex.Message}, please add dmfldr_dll.dll to the working directory first.",ex);
                }
                else
                {
                    throw;
                }
            }
        }

        private static DmBulkCopy BuildSqlBulkCopy<TEntity>(DmConnection conn,DbContext dbCtx, DmTransaction externalTransaction, DmBulkCopyOptions copyOptions) where TEntity : class
        {
            DmBulkCopy bulkCopy = new DmBulkCopy(conn,copyOptions,externalTransaction);
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = BulkInsertUtils.ParseDbProps<TEntity>(entityType);
            bulkCopy.DestinationTableName = entityType.GetSchemaQualifiedTableName();//Schema may be used
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName, columnName);
            }
            return bulkCopy;
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, DmTransaction externalTransaction = null, DmBulkCopyOptions copyOptions = DmBulkCopyOptions.Default) where TEntity : class
        {            
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            using (DmBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((DmConnection)conn, dbCtx,externalTransaction,copyOptions))
            {
                WriteToServer(bulkCopy, dataTable);
            }
        }
    }
}
