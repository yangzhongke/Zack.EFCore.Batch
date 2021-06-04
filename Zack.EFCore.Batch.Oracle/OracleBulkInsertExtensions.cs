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
        private static OracleBulkCopy BuildSqlBulkCopy<TEntity>(OracleConnection conn, DbContext dbCtx) where TEntity : class
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = BulkInsertUtils.ParseDbProps<TEntity>(entityType);

            OracleBulkCopy bulkCopy = new OracleBulkCopy((OracleConnection)conn);

            bulkCopy.DestinationTableName = entityType.GetTableName();//Schema is not supported by MySQL
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName,columnName);
            }
            return bulkCopy;
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenAsync(cancellationToken);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx))
            {
                bulkCopy.WriteToServer(dataTable);
            }            
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.Open();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((OracleConnection)conn, dbCtx))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
