using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage;
using System.Data;
using System.Linq.Expressions;
using System.Text;

namespace Zack.EFCore.Batch.Internal
{
    public class BatchUpdateBuilder<TEntity> where TEntity:class
    {
        private IList<Setter<TEntity>> setters = new List<Setter<TEntity>>();

        private DbContext dbContext;

        private DbSet<TEntity> dbSet;

        private int? skip;
        private int? take;

        public BatchUpdateBuilder(DbContext dbContext,DbSet<TEntity> dbSet)
        {
            this.dbContext = dbContext;
            this.dbSet = dbSet;
        }

        private BatchUpdateBuilder<TEntity> Set(LambdaExpression nameExpr,
            LambdaExpression valueExpr, Type propertType)
        {
            MemberExpression propExpression = nameExpr.Body as MemberExpression;
            string propertyName = propExpression.Member.Name;
            setters.Add(new Setter<TEntity> { Name = nameExpr, Value = valueExpr, PropertyType = propertType, PropertyName = propertyName });
            return this;
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
            var propertyType = typeof(TP);
            return Set(name, value, propertyType);
        }


        /// <summary>
        /// Set(c=>c.Name,"hello")
        /// </summary>
        /// <typeparam name="TP"></typeparam>
        /// <param name="nameExpr"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public BatchUpdateBuilder<TEntity> Set<TP>(Expression<Func<TEntity, TP>> nameExpr,
            TP value)
        {
            var propertyType = typeof(TP);
            //fix https://github.com/yangzhongke/Zack.EFCore.Batch/issues/47
            Expression valueExpr = Expression.Constant(value, propertyType);
            var pExpr = Expression.Parameter(typeof(TEntity));
            var valueLambdaExpr = Expression.Lambda<Func<TEntity,TP>>(valueExpr, pExpr);
            return Set(nameExpr, valueLambdaExpr, propertyType);
        }

        //feature: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/38
        public BatchUpdateBuilder<TEntity> Set(string name,
            object value)
        {
            var propInfo = typeof(TEntity).GetProperty(name);
            Type propType = propInfo.PropertyType;//typeof of property

            var pExpr = Expression.Parameter(typeof(TEntity));
            Type tDelegate = typeof(Func<,>).MakeGenericType(typeof(TEntity),propType);

            var nameExpr = Expression.Lambda(tDelegate,Expression.MakeMemberAccess(pExpr, propInfo), pExpr);
            /*
            Expression valueExpr = Expression.Constant(value,propType);
            if (value!=null&&value.GetType()!= propType)
            {
                valueExpr = Expression.Convert(valueExpr, propType);
            }*/
            Expression valueExpr = Expression.Constant(Convert.ChangeType(value, propType));
            var valueLambdaExpr = Expression.Lambda(tDelegate, valueExpr, pExpr);
            return Set(nameExpr, valueLambdaExpr, propType);
        }

        private string GenerateSQL(Expression<Func<TEntity, bool>> predicate, bool ignoreQueryFilters, out IDictionary<string, object> parameters)
        {
            if (setters.Count <= 0)
            {
                throw new InvalidOperationException("At least a Set() should be used.");
            }       

            ISqlGenerationHelper sqlGenHelpr = this.dbContext.GetService<ISqlGenerationHelper>();

            //every pair of name=value are converted into two columns of Select,
            //for example, Set(b=>b.Age,b=>b.Age+3).Set(b=>b.Name,b=>"tom") is converted into
            //Select(b=>new{b.Age,F1=b.Age+3,b.Name,F2="tom"})
            //combine every two adjacent columns into an assignment expression,
            //for example, select Age,Age+3,Name,"tom" is converted into
            //Age=Age+3,Name="tom"
            var parameter = Expression.Parameter(typeof(TEntity), "e");
            Expression[] initializers = new Expression[setters.Count*2];
            for(var i=0;i<setters.Count;i++)
            {
                var setter = setters[i];
                var propertyType = typeof(object);
                initializers[i * 2] = Expression.Convert(Expression.Invoke(setter.Name, parameter), propertyType);
                initializers[i * 2+1] = Expression.Convert(Expression.Invoke(setter.Value, parameter), propertyType);
            }

            //fix the bug: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/22
            //merge the identical Expressions into one, only tranlsate the unique ones.
            var distinctiveInitializers = initializers
                .Distinct(new ExpressionEqualityComparer()).ToArray();

            // from https://stackoverflow.com/questions/47513122/entity-framework-core-dynamically-build-select-list-with-navigational-propertie
            NewArrayExpression newArrayExp = Expression.NewArrayInit(
                    typeof(object), distinctiveInitializers);
            var selectExpression = Expression.Lambda<Func<TEntity, object>>(newArrayExp, parameter);

            IQueryable<TEntity> queryable = this.dbSet;
            if (ignoreQueryFilters)
            {
                queryable = queryable.IgnoreQueryFilters();
            }
            if (predicate!=null)
            {
                queryable = queryable.Where(predicate);
            }
            if(this.skip!=null)
            {
                queryable = queryable.Skip((int)this.skip);
            }
            if (this.take != null)
            {
                queryable = queryable.Take((int)this.take);
            }
            IQueryable<object> selectQueryable = queryable.Select(selectExpression);
            var parsingResult = selectQueryable.Parse(this.dbContext, ignoreQueryFilters);

            if (distinctiveInitializers.Count()!=parsingResult.ProjectionSQL.Count())
            {
                throw new InvalidOperationException("The count of columns initializersSet and ProjectionSQL should equal.");
            }
            //key is b => b.Title, value is the related SQL,like "b".Title
            Dictionary<Expression,string> initializerSQLDict = new(new ExpressionEqualityComparer());
            for(int i=0;i<distinctiveInitializers.Length;i++)
            {
                Expression expression = distinctiveInitializers.ElementAt(i);
                initializerSQLDict[expression] = parsingResult.ProjectionSQL.ElementAt(i);
            }
            string tableName = sqlGenHelpr.DelimitIdentifier(parsingResult.TableName,parsingResult.Schema);
            StringBuilder sbSQL = new StringBuilder();
            sbSQL.Append("Update ").Append(tableName).Append(" ")
                .Append("SET ");

            var entityType = dbContext.Model.FindEntityType(typeof(TEntity));
            IRelationalTypeMappingSource typeMappingSrc = dbContext.GetService<IRelationalTypeMappingSource>();
            for(int i=0;i<initializers.Length;i=i+2)
            {
                //query SQL of two columns from initializerSQLDict
                string columnName = initializerSQLDict[initializers[i]];
                string columnValue = initializerSQLDict[initializers[i+1]];
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
                if (i < initializers.Length - 2)
                {
                    sbSQL.Append(", ");
                }
            }
            sbSQL.AppendLine();
            /*
            if (parsingResult.FullSQL.Contains("join", StringComparison.OrdinalIgnoreCase))
            {
                string aliasSeparator = parsingResult.QuerySqlGenerator.P_AliasSeparator;
                sbSQL.Append(" WHERE ").Append(BatchUtils.BuildWhereSubQuery(queryable,dbContext, aliasSeparator));
            }
            else
            {
                if (!string.IsNullOrWhiteSpace(parsingResult.PredicateSQL))
                {
                    sbSQL.Append("WHERE ").Append(parsingResult.PredicateSQL);
                }
            }
            */
            string aliasSeparator = parsingResult.QuerySqlGenerator.P_AliasSeparator;
            sbSQL.Append(" WHERE ").Append(BatchUtils.BuildWhereSubQuery(queryable, dbContext, aliasSeparator));
            parameters = parsingResult.Parameters;
            return sbSQL.ToString();
        }

        private Expression<Func<TEntity, bool>> predicate;

        public BatchUpdateBuilder<TEntity> Where(Expression<Func<TEntity, bool>> predicate = null)
        {
            this.predicate = predicate;
            return this;
        }

        public BatchUpdateBuilder<TEntity> Skip(int skipCount)
        {
            this.skip = skipCount;
            return this;
        }

        public BatchUpdateBuilder<TEntity> Take(int takeCount)
        {
            this.take = takeCount;
            return this;
        }

        public async Task<int> ExecuteAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            string sql = GenerateSQL(this.predicate, ignoreQueryFilters,out IDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            await conn.OpenIfNeededAsync(cancellationToken);
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
            string sql = GenerateSQL(this.predicate, ignoreQueryFilters, out IDictionary<string, object> parameters);
            var conn = dbContext.Database.GetDbConnection();
            conn.OpenIfNeeded();
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
