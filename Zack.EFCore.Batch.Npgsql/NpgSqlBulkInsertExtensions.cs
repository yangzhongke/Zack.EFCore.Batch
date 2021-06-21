using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using Zack.EFCore.Batch.Internal;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata;

namespace System.Linq
{
    public static class NpgSqlBulkInsertExtensions
    {
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

        private static NpgsqlBinaryImporter BuildImporter<TEntity>(DbContext dbCtx, NpgsqlConnection pgConn,
            IEnumerable<TEntity> items) where TEntity : class
        {
            var dbSet = dbCtx.Set<TEntity>();
            var entityType = dbSet.EntityType;
            ISqlGenerationHelper sqlGenHelpr = dbCtx.GetService<ISqlGenerationHelper>();
            string destinationTableName = BuildDestTableName(entityType, sqlGenHelpr);
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            string[] colNames = dataTable.Columns.OfType<DataColumn>()
                .Select(c => c.ColumnName).ToArray();
            var delimitedCols = string.Join(",", colNames.Select(n => sqlGenHelpr.DelimitIdentifier(n)));
            var writer = pgConn.BeginBinaryImport($"COPY {destinationTableName} ({delimitedCols}) FROM STDIN (FORMAT BINARY)");
            foreach (DataRow dataRow in dataTable.Rows)
            {
                writer.StartRow();
                foreach (var colName in colNames)
                {
                    writer.Write(dataRow[colName]);
                }
            };
            return writer;
        }

        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DbTransaction tx = conn.BeginTransaction();
            using (tx)
            {
                try
                {
                    using (var writer = BuildImporter<TEntity>(dbCtx, (NpgsqlConnection)conn, items))
                    {
                        writer.Complete();
                        writer.Close();
                    }
                    tx.Commit();
                }
                catch
                {
                    tx.Rollback();
                    throw;
                }
            }                
        }

        public static async Task BulkInsertAsync<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            await conn.OpenIfNeededAsync();
            DbTransaction tx = await conn.BeginTransactionAsync();
            using (tx)
            {
                try
                {
                    using (var writer = BuildImporter<TEntity>(dbCtx, (NpgsqlConnection)conn, items))
                    {
                        await writer.CompleteAsync();
                        await writer.CloseAsync();
                    }
                    await tx.CommitAsync();
                }
                catch
                {
                    await tx.RollbackAsync();
                    throw;
                }
            }
        }
    }
}
