﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.Storage;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.Internal;

namespace System.Linq
{
    public static class BatchEFExtensions
    {
        private static readonly BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.NonPublic;
        private static T? Private<T>(this object obj, string privateField) => (T?)obj?.GetType().GetField(privateField, bindingFlags)?.GetValue(obj);
        private static string GenerateDeleteSQL<TEntity>(DbContext ctx, IQueryable<TEntity> queryable, Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters,
            out List<DbParameter> parameters) where TEntity:class
        {
            if(predicate!=null)
            {
                queryable = queryable.Where(predicate);
            }
            else
            {
                queryable = queryable.Where(e => 1==1);
            }
            if (ignoreQueryFilters)
            {
                queryable = queryable.IgnoreQueryFilters();
            }
            var parsingResult = queryable.Parse(ctx, ignoreQueryFilters);
            ISqlGenerationHelper sqlGenHelpr = ctx.GetService<ISqlGenerationHelper>();
            string tableName = sqlGenHelpr.DelimitIdentifier(parsingResult.TableName,parsingResult.Schema);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("Delete FROM ").Append(tableName);
            if(!string.IsNullOrWhiteSpace(parsingResult.PredicateSQL))
            {
                //fix https://github.com/yangzhongke/Zack.EFCore.Batch/issues/48
                string aliasSeparator = parsingResult.QuerySqlGenerator.P_AliasSeparator;
                sbSQL.Append(" WHERE ").Append(BatchUtils.BuildWhereSubQuery(queryable, ctx, aliasSeparator));
            }
            parameters = parsingResult.Parameters;
            return sbSQL.ToString();
        }

        public static async Task<int> DeleteRangeAsync<TEntity>(this DbContext ctx,
            Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            DbSet<TEntity> dbSet = ctx.Set<TEntity>();
            string sql = GenerateDeleteSQL(ctx, dbSet, predicate, ignoreQueryFilters, out List<DbParameter> parameters);
            ctx.Log(sql);
            return await ExecuteSQLAsync(ctx, sql, parameters, cancellationToken);
        }

        private static async Task<int> ExecuteSQLAsync(DbContext ctx, string sql, List<DbParameter> parameters, CancellationToken cancellationToken)
        {
           return await ctx.Database.ExecuteSqlRawAsync(sql, parameters, cancellationToken);
        }

        public static async Task<int> DeleteRangeAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx,
            Expression<Func<TEntity,bool>> predicate=null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
            where TEntity:class
        {
            string sql = GenerateDeleteSQL(ctx, queryable, predicate, ignoreQueryFilters, out List<DbParameter> parameters);
            ctx.Log(sql);
            return await ExecuteSQLAsync(ctx, sql, parameters, cancellationToken);
        }

        public static int DeleteRange<TEntity>(this DbContext ctx, Expression<Func<TEntity, bool>> predicate=null, bool ignoreQueryFilters = false)
            where TEntity : class
        {
            DbSet<TEntity> dbSet = ctx.Set<TEntity>();
            string sql = GenerateDeleteSQL(ctx, dbSet, predicate, ignoreQueryFilters, out List<DbParameter> parameters);
            ctx.Log(sql);
            return ExecuteSQL(ctx, sql, parameters);
        }

        public static int DeleteRange<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
            where TEntity : class
        {
            string sql = GenerateDeleteSQL(ctx, queryable, predicate, ignoreQueryFilters, out List<DbParameter> parameters);
            ctx.Log(sql);
            return ExecuteSQL(ctx, sql, parameters);
        }

        private static int ExecuteSQL(DbContext ctx, string sql, List<DbParameter> parameters)
        {
            return ctx.Database.ExecuteSqlRawAsync(sql, parameters).Result;
        }

        internal static void ApplyCurrentTransaction(this IDbCommand cmd,DbContext dbContext)
        {
            var tx = dbContext.Database.CurrentTransaction;
            if (tx != null)
            {
                cmd.Transaction = tx.GetDbTransaction();
            }
        }

        internal static void AddParameters(this IDbCommand cmd, DbContext ctx, IDictionary<string, object> parameters)
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
                //fix issue on SQLServer: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/26 
                if (p.Value==null)
                {
                    dbParam.Value = DBNull.Value;
                }
                else
                {
                    dbParam.Value = p.Value;
                }               
                cmd.Parameters.Add(dbParam);
            }
        }

        public static BatchUpdateBuilder<TEntity> BatchUpdate<TEntity>(this DbContext ctx) where TEntity:class
        {
            DbSet<TEntity> dbSet = ctx.Set<TEntity>();
            BatchUpdateBuilder<TEntity> builder = new BatchUpdateBuilder<TEntity>(ctx, dbSet);
            return builder;
        }

        public static BatchUpdateBuilder<TEntity> BatchUpdate<TEntity>(this DbSet<TEntity> dbSet, DbContext ctx) where TEntity : class
        {
            BatchUpdateBuilder<TEntity> builder = new BatchUpdateBuilder<TEntity>(ctx, dbSet);
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
#if (NET7_0_OR_GREATER)
            return default;
#else
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
            var enumerator = queryable.Provider.Execute<IEnumerable>(queryable.Expression).GetEnumerator();
            var queryContextrq = enumerator.Private<RelationalQueryContext>("_relationalQueryContext");
            var parameterValues = queryContextrq?.ParameterValues;
            if (parameterValues != null) {
                foreach (var parameterValue in parameterValues)
                {
                    queryContext.AddParameter(parameterValue.Key, parameterValue.Value);
                }
            }
            selectExpression = _relationalParameterBasedSqlProcessor.Optimize(selectExpression, queryContext.ParameterValues, out bool canCache);

            IQuerySqlGeneratorFactory querySqlGeneratorFactory = ctx.GetService<IQuerySqlGeneratorFactory>();
            IZackQuerySqlGenerator querySqlGenerator = querySqlGeneratorFactory.Create() as IZackQuerySqlGenerator;
            if (querySqlGenerator==null)
            {
                throw new InvalidOperationException("please add dbContext.UseBatchEF() to OnConfiguring first!");
            }
            querySqlGenerator.IsForBatchEF = true;
            var cmd = querySqlGenerator.GetCommand(selectExpression);
            parsingResult.Parameters = new List<DbParameter>();
            foreach (var param in queryContext.ParameterValues)
            {
                parsingResult.Parameters.Add(querySqlGenerator.CreateParameter(param.Key,param.Value));
            }
            //parsingResult.Parameters = queryContext.ParameterValues;
            //  parsingResult.Parameters = ctx.ConvertParameterValues(queryContext.ParameterValues);
            parsingResult.QuerySqlGenerator = querySqlGenerator;
            parsingResult.PredicateSQL = querySqlGenerator.PredicateSQL;
            parsingResult.ProjectionSQL = querySqlGenerator.ProjectionSQL;
            TableExpression tableExpression = selectExpression.Tables[0] as TableExpression;
            parsingResult.TableName = tableExpression.Table.Name;
            parsingResult.Schema = tableExpression.Schema;
            parsingResult.FullSQL = cmd.CommandText;
            return parsingResult;
#endif
        }
    }
}
