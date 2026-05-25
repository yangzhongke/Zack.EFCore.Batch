using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Oracle.ManagedDataAccess.Client;
using Zack.EFCore.BatchInsert.Internal;

namespace Zack.EFCore.BatchInsert.Oracle;

public class OracleBulkInsertExecutor : IBulkInsertExecutor
{
    public bool CanHandle(DbContext dbCtx)
    {
        return dbCtx.Database.GetDbConnection() is OracleConnection;
    }

    public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
        CancellationToken cancellationToken = default)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not OracleConnection oracleConn)
            throw new InvalidOperationException("OracleBulkInsertExecutor can only handle Oracle connections.");
        await conn.OpenIfNeededAsync(cancellationToken);
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(oracleConn, dbCtx, entityType, efEntityType))
        {
            bulkCopy.WriteToServer(dataTable);
        }
    }

    public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not OracleConnection oracleConn)
            throw new InvalidOperationException("OracleBulkInsertExecutor can only handle Oracle connections.");
        conn.OpenIfNeeded();
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        using (var bulkCopy = BuildSqlBulkCopy(oracleConn, dbCtx, entityType, efEntityType))
        {
            bulkCopy.WriteToServer(dataTable);
        }
    }

    private static OracleBulkCopy BuildSqlBulkCopy(OracleConnection conn, DbContext dbCtx, Type entityType,
        IEntityType efEntityType)
    {
        var dbProps = BulkInsertUtils.ParseDbProps(dbCtx, efEntityType, entityType);

        var bulkCopy = new OracleBulkCopy(conn, OracleBulkCopyOptions.Default);

        bulkCopy.DestinationTableName = $"\"{efEntityType.GetTableName()}\"";

        foreach (var dbProp in dbProps)
        {
            var columnName = dbProp.ColumnName;
            bulkCopy.ColumnMappings.Add(columnName, $"\"{columnName}\"");
        }

        return bulkCopy;
    }
}