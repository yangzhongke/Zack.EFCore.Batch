using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Zack.EFCore.Batch.Internal
{
    public class BatchUtils
    {
		public static string GetPKColName<TEntity>(DbSet<TEntity> dbSet) where TEntity : class
		{
			var pkProps = dbSet.EntityType.FindPrimaryKey().Properties;
			if(pkProps.Count!=1)
            {
				throw new ArgumentException("Only entity types with one single primary key are supported.");
            }
			string pkColName = pkProps[0].GetColumnName(StoreObjectIdentifier.SqlQuery(dbSet.EntityType));
			return pkColName;
        }

		public static string UniqueAlias()
        {
			return "V"+Guid.NewGuid().ToString("N");
        }

		public static string BuildWhereSubQuery<TEntity>(IQueryable queryable, DbSet<TEntity> dbSet, string aliasSeparator) where TEntity : class
		{
			SingleQueryingEnumerable<TEntity> queryingEnumerable = queryable.Provider.Execute<IEnumerable>(queryable.Expression) as SingleQueryingEnumerable<TEntity>;
			string subQuerySQL;
			using (var cmd = queryingEnumerable.CreateDbCommand())
			{
				subQuerySQL = cmd.CommandText;
			}

			string tableAlias = BatchUtils.UniqueAlias();
			string pkName = BatchUtils.GetPKColName<TEntity>(dbSet);
			StringBuilder sbSQL = new StringBuilder();
			sbSQL.Append(pkName).Append(" IN(SELECT ").Append(pkName).Append(" FROM (")
				.Append(subQuerySQL).AppendLine($") {aliasSeparator} {tableAlias} )");
			return sbSQL.ToString();
		}

		/// <summary>
		/// exclude the oldSQL from newSQL
		/// Diff("abc","abc12")=="12"
		/// </summary>
		/// <param name="oldSQL"></param>
		/// <param name="newSQL"></param>
		/// <returns></returns>
		public static string Diff(string oldSQL, string newSQL)
		{
			if (!newSQL.StartsWith(oldSQL))
			{
				throw new ArgumentException("!newSQL.StartsWith(oldSQL)", nameof(newSQL));
			}
			return newSQL.Substring(oldSQL.Length);
		}

		//this method is from source code ef core
		public static void GenerateList<T>(IReadOnlyList<T> items, IRelationalCommandBuilder Sql, Action<T> generationAction, Action<IRelationalCommandBuilder> joinAction = null)
		{
			if (joinAction == null)
			{
				joinAction = delegate (IRelationalCommandBuilder isb)
				{
					isb.Append(", ");
				};
			}
			for (int i = 0; i < items.Count; i++)
			{
				if (i > 0)
				{
					joinAction(Sql);
				}
				generationAction(items[i]);
			}
		}

		//this method is from source code ef core
		public static bool IsNonComposedSetOperation(SelectExpression selectExpression)
		{
			if (selectExpression.Offset == null && selectExpression.Limit == null && !selectExpression.IsDistinct && selectExpression.Predicate == null && selectExpression.Having == null && selectExpression.Orderings.Count == 0 && selectExpression.GroupBy.Count == 0 && selectExpression.Tables.Count == 1)
			{
				TableExpressionBase tableExpressionBase = selectExpression.Tables[0];
				SetOperationBase setOperation = tableExpressionBase as SetOperationBase;
				if (setOperation != null && selectExpression.Projection.Count == setOperation.Source1.Projection.Count)
				{
					return selectExpression.Projection.Select(delegate (ProjectionExpression pe, int index)
					{
						ColumnExpression columnExpression = pe.Expression as ColumnExpression;
						if (columnExpression != null && string.Equals(columnExpression.Table.Alias, setOperation.Alias, StringComparison.OrdinalIgnoreCase))
						{
							return string.Equals(columnExpression.Name, setOperation.Source1.Projection[index].Alias, StringComparison.OrdinalIgnoreCase);
						}
						return false;
					}).All((bool e) => e);
				}
			}
			return false;
		}
	}
}
