using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.ExpressionVisitors.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Zack.EFCore.Batch.Internal;

namespace Zack.EFCore.Batch.MySQL.Pomelo.Internal
{
    class ZackQuerySqlGenerator_MySQLPomelo : MySqlQuerySqlGenerator, IZackQuerySqlGenerator
    {
		/// <summary>
		/// columns of the select statement
		/// </summary>
		private List<string> _projectionSQL = new List<string>();

		/// <summary>
		/// if IsForSingleTable=true, ZackQuerySqlGenerator will change the default behavior to capture PredicateSQL and so on.
		/// if IsForSingleTable=false, ZackQuerySqlGenerator will use all the implementations of base class
		/// </summary>
		public bool IsForBatchEF { get; set; }

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
		public ZackQuerySqlGenerator_MySQLPomelo(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper, MySqlSqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
			: base(dependencies, sqlExpressionFactory, options)
		{
			this._sqlGenerationHelper = sqlGenerationHelper;
			this.IsForBatchEF = false;
		}


		protected override Expression VisitSelect(SelectExpression selectExpression)
		{
			if (!IsForBatchEF)
			{
				return base.VisitSelect(selectExpression);
			}
			if (BatchUtils.IsNonComposedSetOperation(selectExpression))
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
				BatchUtils.GenerateList(selectExpression.Projection, Sql, delegate (ProjectionExpression e)
				{
					var oldSQL = Sql.Build().CommandText;//zack's code
					Visit(e);
					string column = BatchUtils.Diff(oldSQL, this.Sql.Build().CommandText); //zack's code
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
				BatchUtils.GenerateList(selectExpression.Tables, Sql, delegate (TableExpressionBase e)
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
				this.PredicateSQL = BatchUtils.Diff(oldSQL, this.Sql.Build().CommandText); //zack's code
			}
			if (selectExpression.GroupBy.Count > 0)
			{
				Sql.AppendLine().Append("GROUP BY ");
				BatchUtils.GenerateList(selectExpression.GroupBy, Sql, delegate (SqlExpression e)
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
			if (IsForBatchEF)
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
			if (IsForBatchEF)
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
