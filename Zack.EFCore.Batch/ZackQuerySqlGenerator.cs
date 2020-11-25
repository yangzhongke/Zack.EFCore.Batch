using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Zack.EFCore.Batch
{
    public class ZackQuerySqlGenerator: QuerySqlGenerator
    {
		/// <summary>
		/// columns of the select statement
		/// </summary>
		private List<string> _projectionSQL = new List<string>();

		/// <summary>
		/// if IsForSingleTable=true, there will be no alias in  PredicateSQL and PredicateSQL
		/// like: select [b].[Id] from [T_Books] AS [b] 
		/// select [Id] from [T_Books]
		/// </summary>
		public bool IsForSingleTable { get; set; } = false;

		public IEnumerable<string> ProjectionSQL 
		{ 
			get
            {
				return this._projectionSQL;
            }
		}

		/// <summary>
		/// the where clause
		/// </summary>
		public string PredicateSQL
        {
			get;
			private set;
        }

		private ISqlGenerationHelper _sqlGenerationHelper;
		public ZackQuerySqlGenerator(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper)
            :base(dependencies)
        {
			this._sqlGenerationHelper = sqlGenerationHelper;
        }

		//from ef core
		private static bool IsNonComposedSetOperation(SelectExpression selectExpression)
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

		//from ef core
		private void GenerateList<T>(IReadOnlyList<T> items, Action<T> generationAction, Action<IRelationalCommandBuilder> joinAction = null)
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

		/// <summary>
		/// exclude the oldSQL from newSQL
		/// Diff("abc","abc12")=="12"
		/// </summary>
		/// <param name="oldSQL"></param>
		/// <param name="newSQL"></param>
		/// <returns></returns>
		private static string Diff(string oldSQL, string newSQL)
        {
			if(!newSQL.StartsWith(oldSQL))
            {
				throw new ArgumentException("!newSQL.StartsWith(oldSQL)",nameof(newSQL));
            }
			return newSQL.Substring(oldSQL.Length);
        }

		protected override Expression VisitSelect(SelectExpression selectExpression)
        {
			if (IsNonComposedSetOperation(selectExpression))
			{
				GenerateSetOperation((SetOperationBase)selectExpression.Tables[0]);
				return selectExpression;
			}
			IDisposable disposable = null;
			if (selectExpression.Alias != null)
			{
				Sql.AppendLine("(");
				disposable = Sql.Indent();
			}
			Sql.Append("SELECT ");
			if (selectExpression.IsDistinct)
			{
				Sql.Append("DISTINCT ");
			}
			GenerateTop(selectExpression);
			if (selectExpression.Projection.Any())
			{
				GenerateList(selectExpression.Projection, delegate (ProjectionExpression e)
				{
					var oldSQL = Sql.Build().CommandText;//zack's code
					Visit(e);
					string column = Diff(oldSQL, this.Sql.Build().CommandText); //zack's code
					this._projectionSQL.Add(column); //zack's code
				});
			}
			else
			{
				Sql.Append("1");
				this._projectionSQL.Add("1");//zack's code
			}
			if (selectExpression.Tables.Any())
			{
				Sql.AppendLine().Append("FROM ");
				GenerateList(selectExpression.Tables, delegate (TableExpressionBase e)
				{
					Visit(e);
				}, delegate (IRelationalCommandBuilder sql)
				{
					sql.AppendLine();
				});
			}
			else
			{
				GeneratePseudoFromClause();
			}
			if (selectExpression.Predicate != null)
			{
				Sql.AppendLine().Append("WHERE ");
				var oldSQL = Sql.Build().CommandText;//zack's code
				Visit(selectExpression.Predicate);
				this.PredicateSQL = Diff(oldSQL, this.Sql.Build().CommandText); //zack's code
			}
			if (selectExpression.GroupBy.Count > 0)
			{
				Sql.AppendLine().Append("GROUP BY ");
				GenerateList(selectExpression.GroupBy, delegate (SqlExpression e)
				{
					Visit(e);
				});
			}
			if (selectExpression.Having != null)
			{
				Sql.AppendLine().Append("HAVING ");
				Visit(selectExpression.Having);
			}
			GenerateOrderings(selectExpression);
			GenerateLimitOffset(selectExpression);
			if (selectExpression.Alias != null)
			{
				disposable.Dispose();
				Sql.AppendLine().Append(")" + AliasSeparator + _sqlGenerationHelper.DelimitIdentifier(selectExpression.Alias));
			}
			return selectExpression;
		}

        protected override Expression VisitColumn(ColumnExpression columnExpression)
        {
			if(IsForSingleTable)
            {
				Sql.Append(_sqlGenerationHelper.DelimitIdentifier(columnExpression.Name));
				return columnExpression;
			}
			else
            {
				return base.VisitColumn(columnExpression);
            }
		}

        protected override Expression VisitTable(TableExpression tableExpression)
        {
			if (IsForSingleTable)
			{
				Sql.Append(_sqlGenerationHelper.DelimitIdentifier(tableExpression.Name));
				return tableExpression;
			}
			else
			{
				return base.VisitTable(tableExpression);
			}			
        }
    }
}
