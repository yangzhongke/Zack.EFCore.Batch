using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.Internal;

namespace System.Linq
{
    public static class MySQLBulkInsertExtensions
    {

        private static MySqlBulkCopy BuildSqlBulkCopy<TEntity>(MySqlConnection conn, DbContext dbCtx,
            MySqlTransaction? transaction = null) where TEntity : class
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            var dbProps = BulkInsertUtils.ParseDbProps<TEntity>(entityType);
            
            MySqlBulkCopy bulkCopy = new MySqlBulkCopy(conn, transaction);

            bulkCopy.DestinationTableName = entityType.GetTableName();//Schema is not supported by MySQL
            int sourceOrdinal = 0;
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(sourceOrdinal, columnName));
                sourceOrdinal++;
            }
            return bulkCopy;
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, MySqlTransaction? transaction = null,CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            MySqlBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((MySqlConnection)conn,dbCtx, transaction);
            await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items, MySqlTransaction? transaction = null, CancellationToken cancellationToken = default) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            MySqlBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((MySqlConnection)conn, dbCtx, transaction);
            bulkCopy.WriteToServer(dataTable);
        }
    }
}
