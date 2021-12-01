using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Zack.EFCore.Batch.Internal
{
    public static class SqlGeneratorUtils
    {
        public static Expression VisitSelect(IZackQuerySqlGenerator sqlGenerator, ISqlGenerationHelper _sqlGenerationHelper, SelectExpression selectExpression)
        {
			if (BatchUtils.IsNonComposedSetOperation(selectExpression))
			{
				sqlGenerator.P_GenerateSetOperation((SetOperationBase)selectExpression.Tables[0]);
				return selectExpression;
			}

			IRelationalCommandBuilder Sql = sqlGenerator.P_Sql;

			IDisposable disposable = null;
			if (selectExpression.Alias != null)
			{
				Sql.AppendLine("(");
				disposable = sqlGenerator.P_Sql.Indent();
			}
			Sql.Append("SELECT ");
			if (selectExpression.IsDistinct)
			{
				Sql.Append("DISTINCT ");
			}
			sqlGenerator.P_GenerateTop(selectExpression);
			if (selectExpression.Projection.Any())
			{
				BatchUtils.GenerateList(selectExpression.Projection, Sql, delegate (ProjectionExpression e)
				{
					var oldSQL = Sql.Build().CommandText;//zack's code
					sqlGenerator.Visit(e);
					var newSQL = Sql.Build().CommandText;
					string column = BatchUtils.Diff(oldSQL, newSQL); //zack's code
					sqlGenerator.ProjectionSQL.Add(column); //zack's code
				});
			}
			else
			{
				/*
				 *await ctx.BatchUpdate<Book>()
                .Set(a => a.Title, a => "测试")
                .Where(a => ctx.Articles.Where(b => b.Id == a.Id && b.Content == "B").Any())
                .ExecuteAsync();
				 */
				//if there is no columns specified,
				//don't add any sql segment to sqlGenerator.ProjectionSQL
				//fix: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/40
				Sql.Append("1");
			}
			/*
			else
			{
				Sql.Append("1");
				sqlGenerator.ProjectionSQL.Add("1");//zack's code
			}*/
			if (selectExpression.Tables.Any())
			{
				Sql.AppendLine().Append("FROM ");
				BatchUtils.GenerateList(selectExpression.Tables, Sql, delegate (TableExpressionBase e)
				{
					sqlGenerator.Visit(e);
				}, delegate (IRelationalCommandBuilder sql)
				{
					sql.AppendLine();
				});
			}
			else
			{
				sqlGenerator.P_GeneratePseudoFromClause();
			}
			if (selectExpression.Predicate != null)
			{
				Sql.AppendLine().Append("WHERE ");
				var oldSQL = Sql.Build().CommandText;//zack's code
				sqlGenerator.Visit(selectExpression.Predicate);
				sqlGenerator.PredicateSQL = BatchUtils.Diff(oldSQL, Sql.Build().CommandText); //zack's code
			}
			if (selectExpression.GroupBy.Count > 0)
			{
				Sql.AppendLine().Append("GROUP BY ");
				BatchUtils.GenerateList(selectExpression.GroupBy, Sql, delegate (SqlExpression e)
				{
					sqlGenerator.Visit(e);
				});
			}
			if (selectExpression.Having != null)
			{
				Sql.AppendLine().Append("HAVING ");
				sqlGenerator.Visit(selectExpression.Having);
			}
			sqlGenerator.P_GenerateOrderings(selectExpression);
			sqlGenerator.P_GenerateLimitOffset(selectExpression);
			if (selectExpression.Alias != null)
			{
				disposable.Dispose();
				Sql.AppendLine().Append(")" + sqlGenerator.P_AliasSeparator + _sqlGenerationHelper.DelimitIdentifier(selectExpression.Alias));
			}
			return selectExpression;
		}
    }
}
