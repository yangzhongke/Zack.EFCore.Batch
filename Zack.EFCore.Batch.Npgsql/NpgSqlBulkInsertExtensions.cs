using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Collections.Generic;
using System.Data;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.Npgsql
{
    public static class NpgSqlBulkInsertExtensions
    {
        
        public static void BulkInsert<TEntity>(this DbContext dbCtx,
            IEnumerable<TEntity> items) where TEntity : class
        {
            var conn = dbCtx.Database.GetDbConnection();
            conn.OpenIfNeeded();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);

            //https://www.wowprices.info/About/bulk-inserting-data-into-postgres-with-net-core
            NpgsqlConnection pgConn = (NpgsqlConnection)conn;
           // pgConn.BeginBinaryImport
        }
    }
}
