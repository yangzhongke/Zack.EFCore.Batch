using System.Data;
using Microsoft.EntityFrameworkCore;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections;
using Zack.EFCore.Batch;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.Oracle
{
	public class OracleBulkInsertExecutor : IBulkInsertExecutor
	{
		private const string ProviderName = "Oracle.EntityFrameworkCore";

		public bool CanHandle(DbContext dbCtx)
		{
			return string.Equals(dbCtx.Database.ProviderName, ProviderName, StringComparison.OrdinalIgnoreCase);
		}

		private static OracleBulkCopy BuildSqlBulkCopy(OracleConnection conn, DbContext dbCtx, Type entityType,
			Microsoft.EntityFrameworkCore.Metadata.IEntityType efEntityType)
		{
			var dbProps = BulkInsertUtils.ParseDbProps(dbCtx, efEntityType, entityType);

			OracleBulkCopy bulkCopy = new OracleBulkCopy(conn, OracleBulkCopyOptions.Default);

			bulkCopy.DestinationTableName = $"\"{efEntityType.GetTableName()}\"";

			foreach (var dbProp in dbProps)
			{
				string columnName = dbProp.ColumnName;
				bulkCopy.ColumnMappings.Add(columnName, $"\"{columnName}\"");
			}
			return bulkCopy;
		}

		public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items, CancellationToken cancellationToken = default)
		{
			var conn = dbCtx.Database.GetDbConnection();
			if (conn is not OracleConnection oracleConn)
			{
				throw new InvalidOperationException("OracleBulkInsertExecutor can only handle Oracle connections.");
			}
			await conn.OpenIfNeededAsync(cancellationToken);
			var efEntityType = dbCtx.Model.FindEntityType(entityType)
				?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
			DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
			using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy(oracleConn, dbCtx, entityType, efEntityType))
			{
				bulkCopy.WriteToServer(dataTable);
			}
		}

		public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
		{
			var conn = dbCtx.Database.GetDbConnection();
			if (conn is not OracleConnection oracleConn)
			{
				throw new InvalidOperationException("OracleBulkInsertExecutor can only handle Oracle connections.");
			}
			conn.OpenIfNeeded();
			var efEntityType = dbCtx.Model.FindEntityType(entityType)
				?? throw new InvalidOperationException($"Cannot resolve EF entity type for {entityType.FullName}.");
			DataTable dataTable = BulkInsertUtils.BuildDataTable(dbCtx, efEntityType, entityType, items);
			using (OracleBulkCopy bulkCopy = BuildSqlBulkCopy(oracleConn, dbCtx, entityType, efEntityType))
			{
				bulkCopy.WriteToServer(dataTable);
			}
		}
	}
}
