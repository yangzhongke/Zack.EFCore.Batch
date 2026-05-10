using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql;
using System;
using System.Collections;
using System.Data;
using Zack.EFCore.Batch;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.Npgsql
{
    public class NpgSqlBulkInsertExecutor : IBulkInsertExecutor
    {
        private const string ProviderName = "Npgsql.EntityFrameworkCore.PostgreSQL";

        public bool CanHandle(DbContext dbCtx)
        {
            return string.Equals(dbCtx.Database.ProviderName, ProviderName, StringComparison.OrdinalIgnoreCase);
        }

        private static string BuildDestTableName(IEntityType entityType, ISqlGenerationHelper sqlGenHelpr)
        {
            //"COPY myschema.t_books" doesn't work, we should use "myschema"."t_books" instead
            string schemaName = entityType.GetSchema();
            string tableName = entityType.GetTableName();
            if(string.IsNullOrWhiteSpace(schemaName))
            {
                return sqlGenHelpr.DelimitIdentifier(tableName);
            }
            else
            {
                return sqlGenHelpr.DelimitIdentifier(schemaName) + "." + sqlGenHelpr.DelimitIdentifier(tableName);
            }
        }

        private static NpgsqlBinaryImporter BuildImporter(DbContext dbCtx, NpgsqlConnection pgConn,
            Type entityType, IEnumerable items)
        {
            var efEntityType = dbCtx.Model.FindEntityType(entityType)
                ?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
            ISqlGenerationHelper sqlGenHelpr = dbCtx.GetService<ISqlGenerationHelper>();
            string destinationTableName = BuildDestTableName(efEntityType, sqlGenHelpr);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
            string[] colNames = dataTable.Columns.OfType<DataColumn>()
                .Select(c => c.ColumnName).ToArray();
            var delimitedCols = string.Join(",", colNames.Select(n => sqlGenHelpr.DelimitIdentifier(n)));
            var writer = pgConn.BeginBinaryImport($"COPY {destinationTableName} ({delimitedCols}) FROM STDIN (FORMAT BINARY)");
            foreach (DataRow dataRow in dataTable.Rows)
            {
                writer.StartRow();
                foreach (var colName in colNames)
                {
                    object value = dataRow[colName];
                    writer.Write(value);
                }
            };
            return writer;
        }

        public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
        {
            var conn = dbCtx.Database.GetDbConnection();
            if (conn is not NpgsqlConnection npgsqlConn)
            {
                throw new InvalidOperationException("NpgSqlBulkInsertExecutor can only handle Npgsql connections.");
            }
            conn.OpenIfNeeded();
            using (var writer = BuildImporter(dbCtx, npgsqlConn, entityType, items))
            {
                writer.Complete();
                writer.Close();
            }
        }

        public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items, CancellationToken cancellationToken = default)
        {
            var conn = dbCtx.Database.GetDbConnection();
            if (conn is not NpgsqlConnection npgsqlConn)
            {
                throw new InvalidOperationException("NpgSqlBulkInsertExecutor can only handle Npgsql connections.");
            }
            await conn.OpenIfNeededAsync(cancellationToken);
            using (var writer = BuildImporter(dbCtx, npgsqlConn, entityType, items))
            {
                await writer.CompleteAsync(cancellationToken);
                await writer.CloseAsync();
            }
        }
    }
}
