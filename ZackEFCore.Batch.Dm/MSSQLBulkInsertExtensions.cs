using Dm;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.DM
{
    public class DmBulkInsertExecutor : IBulkInsertExecutor
    {
        public bool CanHandle(DbContext dbCtx)
        {
            var providerName = dbCtx.Database.ProviderName;
            return providerName != null &&
                (providerName.IndexOf("EntityFrameworkCore.Dm", StringComparison.OrdinalIgnoreCase) >= 0
                || string.Equals(providerName, "Dm.EntityFrameworkCore", StringComparison.OrdinalIgnoreCase));
        }

        public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items, CancellationToken cancellationToken = default)
        {
            var conn = dbCtx.Database.GetDbConnection();
            if (conn is not DmConnection dmConn)
            {
                throw new InvalidOperationException("DmBulkInsertExecutor can only handle DM connections.");
            }
            await conn.OpenIfNeededAsync(cancellationToken);
            var efEntityType = dbCtx.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
            using (DmBulkCopy bulkCopy = BuildSqlBulkCopy(dmConn, dbCtx, entityType, efEntityType))
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

        private static DmBulkCopy BuildSqlBulkCopy(DmConnection conn, DbContext dbCtx, Type entityType, Microsoft.EntityFrameworkCore.Metadata.IEntityType efEntityType)
        {
            DmBulkCopy bulkCopy = new DmBulkCopy(conn, DmBulkCopyOptions.Default, null);
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
            if (conn is not DmConnection dmConn)
            {
                throw new InvalidOperationException("DmBulkInsertExecutor can only handle DM connections.");
            }
            conn.OpenIfNeeded();
            var efEntityType = dbCtx.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
            using (DmBulkCopy bulkCopy = BuildSqlBulkCopy(dmConn, dbCtx, entityType, efEntityType))
            {
                WriteToServer(bulkCopy, dataTable);
            }
        }
    }
}
