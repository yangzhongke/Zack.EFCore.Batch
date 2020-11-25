using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Zack.EFCore.Batch
{
    public class BatchUpdateBuilder<TEntity> where TEntity:class
    {
        private IList<Setter<TEntity>> setters = new List<Setter<TEntity>>();

        private DbContext dbContext;

        public BatchUpdateBuilder(DbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public BatchUpdateBuilder<TEntity> Set(Expression<Func<TEntity, object>> name, 
            Expression<Func<TEntity, object>> value)
        {
            setters.Add(new Setter<TEntity> { Name=name,Value=value});
            return this;
        }

        /// <summary>
        /// from https://stackoverflow.com/questions/47513122/entity-framework-core-dynamically-build-select-list-with-navigational-propertie
        /// </summary>
        /// <param name="members"></param>
        /// <returns></returns>
        public Expression<Func<TEntity, object>> ToDynamicColumns(IEnumerable<LambdaExpression> members)
        {
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            return Expression.Lambda<Func<TEntity, object>>(
                Expression.NewArrayInit(
                    typeof(object),
                    members.Select(m => Expression.Convert(Expression.Invoke(m, parameter), typeof(object))
                    )
                ), parameter);
        }

        private string GenerateSQL(Expression<Func<TEntity, bool>> predicate,out IReadOnlyDictionary<string, object> parameters)
        {
            if (setters.Count <= 0)
            {
                throw new InvalidOperationException("At least a Set() should be used.");
            }

            ISqlGenerationHelper sqlGenHelpr = this.dbContext.GetService<ISqlGenerationHelper>();

            List<LambdaExpression> columnExpressions = new List<LambdaExpression>();
            foreach (var setter in this.setters)
            {
                columnExpressions.Add(setter.Name);
                columnExpressions.Add(setter.Value);
            }
            var selectExpression = ToDynamicColumns(columnExpressions);

            IQueryable<TEntity> queryable = this.dbContext.Set<TEntity>();
            if(predicate!=null)
            {
                queryable = queryable.Where(predicate);
            }
            IQueryable<object> selectQueryable = queryable.Select(selectExpression);
            var parsingResult = selectQueryable.Parse(this.dbContext);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("Update ").Append(sqlGenHelpr.DelimitIdentifier(parsingResult.TableName)).Append(" ")
                .Append("SET ");
            var columns = parsingResult.ProjectionSQL.ToArray();
            if (columns.Length % 2 != 0)
            {
                throw new InvalidOperationException("The count of columns should be even.");
            }
            for (int i = 0; i < columns.Length; i = i + 2)
            {
                sbSQL.Append(columns[i]).Append(" = ").Append(columns[i + 1]);
                if (i < columns.Length - 2)
                {
                    sbSQL.Append(", ");
                }
            }
            sbSQL.AppendLine();
            if (!string.IsNullOrWhiteSpace(parsingResult.PredicateSQL))
            {
                sbSQL.Append("WHERE ").Append(parsingResult.PredicateSQL);
            }
            parameters = parsingResult.Parameters;
            return sbSQL.ToString();
        }

        public async Task<int> ExecuteAsync(Expression<Func<TEntity, bool>> predicate=null)
        {
            string sql = GenerateSQL(predicate,out IReadOnlyDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameters(parameters);
                return await cmd.ExecuteNonQueryAsync();
            }
        }

        public int Execute(Expression<Func<TEntity, bool>> predicate = null)
        {
            string sql = GenerateSQL(predicate, out IReadOnlyDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            using (var cmd = conn.CreateCommand())
            {
                cmd.CommandText = sql;
                cmd.AddParameters(parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }

    class Setter<TEntity>
    {
        public Expression<Func<TEntity, object>> Name { get; set; }
        public Expression<Func<TEntity, object>> Value { get; set; }
    }
}
