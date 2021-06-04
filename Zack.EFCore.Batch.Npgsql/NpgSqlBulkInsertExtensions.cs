using Microsoft.EntityFrameworkCore;
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
            conn.Open();
            DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx.Set<TEntity>(), items);
            
            MySqlBulkCopy bulkCopy = BuildSqlBulkCopy<TEntity>((MySqlConnection)conn, dbCtx, transaction);
            bulkCopy.WriteToServer(dataTable);
        }
    }
}
