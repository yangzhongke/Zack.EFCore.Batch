using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections.Generic;
using System.Data;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Zack.EFCore.Batch.Internal;

namespace System.Linq
{
    public static class BatchEFExtensions
    {
        private static string GenerateDeleteSQL<TEntity>(DbContext ctx, Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters,
            out IReadOnlyDictionary<string, object> parameters) where TEntity:class
        {
            IQueryable<TEntity> queryable = ctx.Set<TEntity>();
            if(predicate!=null)
            {
                queryable = queryable.Where(predicate);
            }
            else
            {
                queryable = queryable.Where(e => 1==1);
            }
            var parsingResult = queryable.Parse(ctx, ignoreQueryFilters);
            ISqlGenerationHelper sqlGenHelpr = ctx.GetService<ISqlGenerationHelper>();
            string tableName = sqlGenHelpr.DelimitIdentifier(parsingResult.TableName);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("Delete FROM ").Append(tableName);
            if(!string.IsNullOrWhiteSpace(parsingResult.PredicateSQL))
            {
                sbSQL.Append(" WHERE ").Append(parsingResult.PredicateSQL);
            }
            parameters = parsingResult.Parameters;
            return sbSQL.ToString();
        }
        public static async Task<int> DeleteRangeAsync<TEntity>(this DbContext ctx,
            Expression<Func<TEntity,bool>> predicate=null, bool ignoreQueryFilters = false)
            where TEntity:class
        {
            string sql = GenerateDeleteSQL(ctx, predicate, ignoreQueryFilters, out IReadOnlyDictionary<string, object> parameters);
            ctx.Log(sql);
            var conn = ctx.Database.GetDbConnection();
            if(conn.State!= ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.ApplyCurrentTransaction(ctx);      
                cmd.CommandText = sql;
                cmd.AddParameters(ctx,parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public static int DeleteRange<TEntity>(this DbContext ctx, Expression<Func<TEntity, bool>> predicate=null, bool ignoreQueryFilters = false)
            where TEntity : class
        {
            string sql = GenerateDeleteSQL(ctx, predicate, ignoreQueryFilters, out IReadOnlyDictionary<string, object> parameters);
            ctx.Log(sql);
            var conn = ctx.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.ApplyCurrentTransaction(ctx);
                cmd.CommandText = sql;
                cmd.AddParameters(ctx, parameters);
                return cmd.ExecuteNonQuery();
            }
        }

        internal static void ApplyCurrentTransaction(this IDbCommand cmd,DbContext dbContext)
        {
            var tx = dbContext.Database.CurrentTransaction;
            if (tx != null)
            {
                cmd.Transaction = tx.GetDbTransaction();
            }
        }

        internal static void AddParameters(this IDbCommand cmd, DbContext ctx, IReadOnlyDictionary<string, object> parameters)
        {
            var typeMapping = ctx.GetService<IRelationalTypeMappingSource>();
            foreach (var p in parameters)
            {
                if(p.Value!=null)
                {
                    var mappedType = typeMapping.FindMapping(p.Value.GetType());
                    //the parameter type is not supported by underlying database.
                    //the value may be EF.Functions.ContainsOrEqual, int[] that have been translated into SQL clause.
                    //like Where(m => EF.Functions.ContainsOrEqual(m.IPv4.Value, ip)), and Where(p=>ids.Contains(p.Id)),
                    //so it's ignored.
                    if (mappedType==null)
                    {
                        continue;
                    }
                }                
                var dbParam = cmd.CreateParameter();
                dbParam.ParameterName = p.Key;
                dbParam.Value = p.Value;
                cmd.Parameters.Add(dbParam);
            }
        }

        public static BatchUpdateBuilder<TEntity> BatchUpdate<TEntity>(this DbContext ctx) where TEntity:class
        {
            BatchUpdateBuilder<TEntity> builder = new BatchUpdateBuilder<TEntity>(ctx);
            return builder;
        }

        /// <summary>
        /// parse select statement of queryable
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="queryable"></param>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public static SelectParsingResult Parse<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx,bool ignoreQueryFilters) where TEntity:class
        {
            if(ignoreQueryFilters)
            {
                queryable = queryable.IgnoreQueryFilters();
            }            
            SelectParsingResult parsingResult = new SelectParsingResult();
            Expression query = queryable.Expression;
            var databaseDependencies = ctx.GetService<DatabaseDependencies>();
            IQueryTranslationPreprocessorFactory _queryTranslationPreprocessorFactory = ctx.GetService<IQueryTranslationPreprocessorFactory>();
            IQueryableMethodTranslatingExpressionVisitorFactory _queryableMethodTranslatingExpressionVisitorFactory = ctx.GetService<IQueryableMethodTranslatingExpressionVisitorFactory>();
            IQueryTranslationPostprocessorFactory _queryTranslationPostprocessorFactory = ctx.GetService<IQueryTranslationPostprocessorFactory>();
            QueryCompilationContext queryCompilationContext = databaseDependencies.QueryCompilationContextFactory.Create(true);

            IDiagnosticsLogger<DbLoggerCategory.Query> logger = ctx.GetService<IDiagnosticsLogger<DbLoggerCategory.Query>>();
            QueryContext queryContext = ctx.GetService<IQueryContextFactory>().Create();
            QueryCompiler queryComipler = ctx.GetService<IQueryCompiler>() as QueryCompiler;
            //parameterize determines if it will use "Declare" or not
            MethodCallExpression methodCallExpr1 = queryComipler.ExtractParameters(query, queryContext, logger, parameterize: true) as MethodCallExpression;
            QueryTranslationPreprocessor queryTranslationPreprocessor = _queryTranslationPreprocessorFactory.Create(queryCompilationContext);
            MethodCallExpression methodCallExpr2 = queryTranslationPreprocessor.Process(methodCallExpr1) as MethodCallExpression;
            QueryableMethodTranslatingExpressionVisitor queryableMethodTranslatingExpressionVisitor =
                _queryableMethodTranslatingExpressionVisitorFactory.Create(queryCompilationContext);
            ShapedQueryExpression shapedQueryExpression1 = queryableMethodTranslatingExpressionVisitor.Visit(methodCallExpr2) as ShapedQueryExpression;
            QueryTranslationPostprocessor queryTranslationPostprocessor= _queryTranslationPostprocessorFactory.Create(queryCompilationContext);
            ShapedQueryExpression shapedQueryExpression2 = queryTranslationPostprocessor.Process(shapedQueryExpression1) as ShapedQueryExpression;
        
            IRelationalParameterBasedSqlProcessorFactory _relationalParameterBasedSqlProcessorFactory = 
                ctx.GetService<IRelationalParameterBasedSqlProcessorFactory>();
            RelationalParameterBasedSqlProcessor _relationalParameterBasedSqlProcessor = _relationalParameterBasedSqlProcessorFactory.Create(true);

            SelectExpression selectExpression = (SelectExpression)shapedQueryExpression2.QueryExpression;
            selectExpression = _relationalParameterBasedSqlProcessor.Optimize(selectExpression, queryContext.ParameterValues, out bool canCache);
            IQuerySqlGeneratorFactory querySqlGeneratorFactory = ctx.GetService<IQuerySqlGeneratorFactory>();
            IZackQuerySqlGenerator querySqlGenerator = querySqlGeneratorFactory.Create() as IZackQuerySqlGenerator;
            if (querySqlGenerator==null)
            {
                throw new InvalidOperationException("please add dbContext.UseBatchEF() to OnConfiguring first!");
            }
            querySqlGenerator.IsForBatchEF = true;
            querySqlGenerator.GetCommand(selectExpression);
            parsingResult.Parameters = queryContext.ParameterValues;
            parsingResult.PredicateSQL = querySqlGenerator.PredicateSQL;
            parsingResult.ProjectionSQL = querySqlGenerator.ProjectionSQL;
            TableExpression tableExpression = selectExpression.Tables[0] as TableExpression;
            parsingResult.TableName = tableExpression.Table.Name;

            return parsingResult;
        }
    }
}
