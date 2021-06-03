using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Zack.EFCore.Batch.Internal
{
    public class BatchUpdateBuilder<TEntity> where TEntity:class
    {
        private IList<Setter<TEntity>> setters = new List<Setter<TEntity>>();

        private DbContext dbContext;

        private DbSet<TEntity> dbSet;

        public BatchUpdateBuilder(DbContext dbContext,DbSet<TEntity> dbSet)
        {
            this.dbContext = dbContext;
            this.dbSet = dbSet;
        }

        /// <summary>
        /// name is the expression of property's name, and value is the expression of the value
        /// </summary>
        /// <param name="name">something like: b=>b.Age</param>
        /// <param name="value">something like: b=>b.Age+1</param>
        /// <returns></returns>
        public BatchUpdateBuilder<TEntity> Set<TP>(Expression<Func<TEntity, TP>> name, 
            Expression<Func<TEntity, TP>> value)
        {
            MemberExpression propExpression = name.Body as MemberExpression;
            string propertyName = propExpression.Member.Name;
            setters.Add(new Setter<TEntity> { Name=name,Value=value,PropertyType=typeof(TP),PropertyName= propertyName });
            return this;
        }

        private string GenerateSQL(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters, out IReadOnlyDictionary<string, object> parameters)
        {
            if (setters.Count <= 0)
            {
                throw new InvalidOperationException("At least a Set() should be used.");
            }       

            ISqlGenerationHelper sqlGenHelpr = this.dbContext.GetService<ISqlGenerationHelper>();

            //every pair of name=value are converted into two columns of Select,
            //for example, Set(b=>b.Age,b=>b.Age+3).Set(b=>b.Name,b=>"tom") is converted into
            //Select(b=>new{b.Age,F1=b.Age+3,b.Name,F2="tom"})     
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            Expression[] initializers = new Expression[setters.Count*2];
            for(var i=0;i<setters.Count;i++)
            {
                var setter = setters[i];
                var propertyType = typeof(object);
                initializers[i * 2] = Expression.Convert(Expression.Invoke(setter.Name, parameter), propertyType);
                initializers[i * 2+1] = Expression.Convert(Expression.Invoke(setter.Value, parameter), propertyType);
            }

            // from https://stackoverflow.com/questions/47513122/entity-framework-core-dynamically-build-select-list-with-navigational-propertie
            NewArrayExpression newArrayExp = Expression.NewArrayInit(
                    typeof(object), initializers);
            var selectExpression = Expression.Lambda<Func<TEntity, object>>(newArrayExp, parameter);

            //IQueryable <TEntity> queryable = this.dbContext.Set<TEntity>();
            IQueryable<TEntity> queryable = this.dbSet;
            if (predicate!=null)
            {
                queryable = queryable.Where(predicate);
            }
            IQueryable<object> selectQueryable = queryable.Select(selectExpression);
            var parsingResult = selectQueryable.Parse(this.dbContext, ignoreQueryFilters);
            string tableName = sqlGenHelpr.DelimitIdentifier(parsingResult.TableName,parsingResult.Schema);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("Update ").Append(tableName).Append(" ")
                .Append("SET ");
            var columns = parsingResult.ProjectionSQL.ToArray();
            if (columns.Length % 2 != 0)
            {
                throw new InvalidOperationException("The count of columns should be even.");
            }
            //combine every two adjacent columns into an assignment expression,
            //for example, select Age,Age+3,Name,"tom" is converted into
            //Age=Age+3,Name="tom"
            var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
            IRelationalTypeMappingSource typeMappingSrc = dbContext.GetService<IRelationalTypeMappingSource>();
            for (int i = 0; i < columns.Length; i = i + 2)
            {
                string columnName = columns[i];
                string columnValue = columns[i + 1];
                var setter = setters[i / 2];
                var property = entityType.GetProperty(setter.PropertyName);
                var valueConverter = property.GetValueConverter();
                
                //fix bug start: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/4
                if (valueConverter!=null&&setter.PropertyType.IsEnum)
                {
                    if(!(setter.Value.Body is ConstantExpression))
                    {
                        throw new NotSupportedException("Only assignment of constant values to enumerated types is supported currently.");
                    }
                    //when expression is put in Select(u=>u.Status), it will not be converted by converter,
                    //so I need convert it manually.
                    int intValue = Convert.ToInt32(columnValue);
                    Enum enumValue = EnumHelper.FromInt(setter.PropertyType, intValue);
                    object convertedValue = valueConverter.ConvertToProvider(enumValue);
                    var typeMapping = typeMappingSrc.FindMapping(property);
                    //single quote string const
                    columnValue = typeMapping.GenerateProviderValueSqlLiteral(convertedValue);
                }
                //fix bug end

                sbSQL.Append(columnName).Append(" = ").Append(columnValue);
                if (i < columns.Length - 2)
                {
                    sbSQL.Append(", ");
                }
            }
            sbSQL.AppendLine();
            if (parsingResult.FullSQL.Contains("join", StringComparison.OrdinalIgnoreCase))
            {
                string aliasSeparator = parsingResult.QuerySqlGenerator.P_AliasSeparator;
                sbSQL.Append(" WHERE ").Append(BatchUtils.BuildWhereSubQuery(queryable,dbSet, aliasSeparator));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(parsingResult.PredicateSQL))
                {
                    sbSQL.Append("WHERE ").Append(parsingResult.PredicateSQL);
                }
            }
            
            parameters = parsingResult.Parameters;
            return sbSQL.ToString();
        }

        private Expression<Func<TEntity, bool>> predicate;

        public BatchUpdateBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate = null)
        {
            this.predicate = predicate;
            return this;
        }

        public async Task<int> ExecuteAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            string sql = GenerateSQL(this.predicate, ignoreQueryFilters,out IReadOnlyDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                await conn.OpenAsync();
            }
            this.dbContext.Log($"Zack.EFCore.Batch: Sql={sql}");
            using (var cmd = conn.CreateCommand())
            {
                cmd.ApplyCurrentTransaction(this.dbContext);
                cmd.CommandText = sql;
                cmd.AddParameters(dbContext, parameters);
                return await cmd.ExecuteNonQueryAsync(cancellationToken);
            }
        }

        public int Execute(bool ignoreQueryFilters=false)
        {
            string sql = GenerateSQL(this.predicate, ignoreQueryFilters, out IReadOnlyDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open)
            {
                conn.Open();
            }
            this.dbContext.Log($"Zack.EFCore.Batch: Sql={sql}");
            using (var cmd = conn.CreateCommand())
            {
                cmd.ApplyCurrentTransaction(this.dbContext);
                cmd.CommandText = sql;
                cmd.AddParameters(dbContext,parameters);
                return cmd.ExecuteNonQuery();
            }
        }
    }

    class Setter<TEntity>
    {
        public LambdaExpression Name { get; set; }
        public LambdaExpression Value { get; set; }
        public Type PropertyType { get; set; }
        public string PropertyName { get; set; }
    }
}
