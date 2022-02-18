using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Zack.EFCore.Batch.Internal
{
    public class BatchUpdateBuilder<TEntity> where TEntity : class
    {
        private IList<Setter<TEntity>> setters = new List<Setter<TEntity>>();

        private DbContext dbContext;

        private DbSet<TEntity> dbSet;

        private int? skip;
        private int? take;

        public BatchUpdateBuilder(DbContext dbContext, DbSet<TEntity> dbSet)
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
            var valueLambdaExpr = Expression.Lambda<Func<TEntity, TP>>(valueExpr, pExpr);
            return Set(nameExpr, valueLambdaExpr, propertyType);
        }

        //feature: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/38
        public BatchUpdateBuilder<TEntity> Set(string name,
            object value)
        {
            var propInfo = typeof(TEntity).GetProperty(name);
            Type propType = propInfo.PropertyType;//typeof of property

            var pExpr = Expression.Parameter(typeof(TEntity));
            Type tDelegate = typeof(Func<,>).MakeGenericType(typeof(TEntity), propType);

            var nameExpr = Expression.Lambda(tDelegate, Expression.MakeMemberAccess(pExpr, propInfo), pExpr);
            Expression valueExpr = Expression.Constant(value);
            if (value != null && value.GetType() != propType)
            {
                valueExpr = Expression.Convert(valueExpr, propType);
            }
            var valueLambdaExpr = Expression.Lambda(tDelegate, valueExpr, pExpr);
            return Set(nameExpr, valueLambdaExpr, propType);
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

        private void ModifyData(bool ignoreQueryFilters = false)
        {
            IQueryable<TEntity> items = this.dbSet;
            if(ignoreQueryFilters)
            {
                items = items.IgnoreQueryFilters();
            }
            if(predicate!=null)
            {
                items = items.Where(predicate);
            }
            if(skip!=null)
            {
                items = items.Skip((int)skip);
            }
            if (take != null)
            {
                items = items.Take((int)take);
            }
            foreach(var setter in setters)
            {
                PropertyInfo propInfo = typeof(TEntity).GetProperty(setter.PropertyName);
                Delegate valueFunc = setter.Value.Compile();
                foreach (var item in items)
                {
                    object value = valueFunc.DynamicInvoke(item);
                    propInfo.SetValue(item, value);
                }
            }
        }


        public Task<int> ExecuteAsync(bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
        {
            ModifyData(ignoreQueryFilters);
            return this.dbContext.SaveChangesAsync(cancellationToken);
        }

        public int Execute(bool ignoreQueryFilters = false)
        {
            ModifyData(ignoreQueryFilters);
            return this.dbContext.SaveChanges();
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
