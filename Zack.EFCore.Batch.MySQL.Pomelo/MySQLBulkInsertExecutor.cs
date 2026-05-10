using System.Collections;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using MySqlConnector;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.MySQL.Pomelo;

public class MySQLBulkInsertExecutor : IBulkInsertExecutor
{
    private const string ProviderName = "Pomelo.EntityFrameworkCore.MySql";

    public bool CanHandle(DbContext dbCtx)
    {
        return string.Equals(dbCtx.Database.ProviderName, ProviderName, StringComparison.OrdinalIgnoreCase);
    }

    public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
        CancellationToken cancellationToken = default)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not MySqlConnection mySqlConn)
            throw new InvalidOperationException("MySQLBulkInsertExecutor can only handle MySQL connections.");
        await conn.OpenIfNeededAsync(cancellationToken);
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        var bulkCopy = BuildSqlBulkCopy(mySqlConn, dbCtx, entityType, efEntityType);
        await bulkCopy.WriteToServerAsync(dataTable, cancellationToken);
    }

    public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
    {
        var conn = dbCtx.Database.GetDbConnection();
        if (conn is not MySqlConnection mySqlConn)
            throw new InvalidOperationException("MySQLBulkInsertExecutor can only handle MySQL connections.");
        conn.OpenIfNeeded();
        var efEntityType = dbCtx.Model.FindEntityType(entityType)
                           ?? throw new InvalidOperationException(
                               $"Cannot resolve EF entity type for {entityType.FullName}.");
        var dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
        var bulkCopy = BuildSqlBulkCopy(mySqlConn, dbCtx, entityType, efEntityType);
        bulkCopy.WriteToServer(dataTable);
    }


    private static MySqlBulkCopy BuildSqlBulkCopy(MySqlConnection conn, DbContext dbCtx,
        Type entityType, IEntityType efEntityType)
    {
        var dbProps = BulkInsertUtils.ParseDbProps(dbCtx, efEntityType, entityType);

        var bulkCopy = new MySqlBulkCopy(conn);

        bulkCopy.DestinationTableName = efEntityType.GetTableName(); //Schema is not supported by MySQL
        var sourceOrdinal = 0;
        foreach (var dbProp in dbProps)
        {
            var columnName = dbProp.ColumnName;
            bulkCopy.ColumnMappings.Add(new MySqlBulkCopyColumnMapping(sourceOrdinal, columnName));
            sourceOrdinal++;
        }

        return bulkCopy;
    }
}