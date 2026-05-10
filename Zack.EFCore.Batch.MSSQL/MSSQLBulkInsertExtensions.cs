using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Data;
using Zack.EFCore.Batch;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.MSSQL
{
    public class MSSQLBulkInsertExecutor : IBulkInsertExecutor
    {
        private const string ProviderName = "Microsoft.EntityFrameworkCore.SqlServer";

        public bool CanHandle(DbContext dbCtx)
        {
            return string.Equals(dbCtx.Database.ProviderName, ProviderName, StringComparison.OrdinalIgnoreCase);
        }

        public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items, CancellationToken cancellationToken = default)
        {
            var conn = dbCtx.Database.GetDbConnection();
            if (conn is not SqlConnection sqlConn)
            {
                throw new InvalidOperationException("MSSQLBulkInsertExecutor can only handle SQL Server connections.");
            }
            await conn.OpenIfNeededAsync(cancellationToken);
            var efEntityType = dbCtx.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
            using (SqlBulkCopy bulkCopy = BuildSqlBulkCopy(sqlConn, dbCtx, entityType, efEntityType))
            {                
                await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
            }
        }

        private static SqlBulkCopy BuildSqlBulkCopy(SqlConnection conn, DbContext dbCtx, Type entityType, Microsoft.EntityFrameworkCore.Metadata.IEntityType efEntityType)
        {
            SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, null);
            var dbProps = BulkInsertUtils.ParseDbProps(dbCtx, efEntityType, entityType);
            bulkCopy.DestinationTableName = efEntityType.GetSchemaQualifiedTableName();//Schema may be used
            foreach (var dbProp in dbProps)
            {
                string columnName = dbProp.ColumnName;
                bulkCopy.ColumnMappings.Add(columnName, columnName);
            }
            return bulkCopy;
        }

        public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
        {            
            var conn = dbCtx.Database.GetDbConnection();
            if (conn is not SqlConnection sqlConn)
            {
                throw new InvalidOperationException("MSSQLBulkInsertExecutor can only handle SQL Server connections.");
            }
            conn.OpenIfNeeded();
            var efEntityType = dbCtx.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
            using (SqlBulkCopy bulkCopy = BuildSqlBulkCopy(sqlConn, dbCtx, entityType, efEntityType))
            {
                bulkCopy.WriteToServer(dataTable);
            }
        }
    }
}
