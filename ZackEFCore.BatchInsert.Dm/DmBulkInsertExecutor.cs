using System.Collections;
using System.Data;
using Dm;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zack.EFCore.BatchInsert.Internal;

namespace Zack.EFCore.BatchInsert.DM;

public class DmBulkInsertExecutor : IBulkInsertExecutor
{
    public bool CanHandle(DbContext dbCtx)
    {
        return dbCtx.Database.GetDbConnection() is DmConnection;
    }

    public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
        CancellationToken cancellationToken = default)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not DmConnection dmConn)
            throw new InvalidOperationException("DmBulkInsertExecutor can only handle DM connections.");
        await conn.OpenIfNeededAsync(cancellationToken);
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(dmConn, dbCtx, entityType, efEntityType))
        {
            await Task.Run(() => { WriteToServer(bulkCopy, dataTable); }, cancellationToken);
        }
    }

    public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not DmConnection dmConn)
            throw new InvalidOperationException("DmBulkInsertExecutor can only handle DM connections.");
        conn.OpenIfNeeded();
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(dmConn, dbCtx, entityType, efEntityType))
        {
            WriteToServer(bulkCopy, dataTable);
        }
    }

    private static void WriteToServer(DmBulkCopy bulkCopy, DataTable dataTable)
    {
        try
        {
            bulkCopy.WriteToServer(dataTable);
        }
        catch (DmException ex)
        {
            if (ex.Message.Contains("fastloading dll not loading"))
                throw new Exception($"{ex.Message}, please add dmfldr_dll.dll to the working directory first.", ex);

            throw;
        }
    }

    private static DmBulkCopy BuildSqlBulkCopy(DmConnection conn, DbContext dbCtx, Type entityType,
        IEntityType efEntityType)
    {
        var bulkCopy = new DmBulkCopy(conn, DmBulkCopyOptions.Default, null);
        var dbProps = BulkInsertUtils.ParseDbProps(dbCtx, efEntityType, entityType);
        bulkCopy.DestinationTableName = efEntityType.GetSchemaQualifiedTableName(); //Schema may be used
        foreach (var dbProp in dbProps)
        {
            var columnName = dbProp.ColumnName;
            bulkCopy.ColumnMappings.Add(columnName, columnName);
        }

        return bulkCopy;
    }
}