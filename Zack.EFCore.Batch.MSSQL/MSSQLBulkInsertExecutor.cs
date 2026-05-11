using System.Collections;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.MSSQL;

public class MSSQLBulkInsertExecutor : IBulkInsertExecutor
{
    public bool CanHandle(DbContext dbCtx)
    {
        return dbCtx.Database.GetDbConnection() is SqlConnection;
    }

    public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
        CancellationToken cancellationToken = default)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not SqlConnection sqlConn)
            throw new InvalidOperationException("MSSQLBulkInsertExecutor can only handle SQL Server connections.");
        await conn.OpenIfNeededAsync(cancellationToken);
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(sqlConn, dbCtx, entityType, efEntityType))
        {
            await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
        }
    }

    public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not SqlConnection sqlConn)
            throw new InvalidOperationException("MSSQLBulkInsertExecutor can only handle SQL Server connections.");
        conn.OpenIfNeeded();
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(sqlConn, dbCtx, entityType, efEntityType))
        {
            bulkCopy.WriteToServer(dataTable);
        }
    }

    private static SqlBulkCopy BuildSqlBulkCopy(SqlConnection conn, DbContext dbCtx, Type entityType,
        IEntityType efEntityType)
    {
        var bulkCopy = new SqlBulkCopy(conn, SqlBulkCopyOptions.Default, null);
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