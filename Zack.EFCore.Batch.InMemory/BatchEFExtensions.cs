using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Zack.EFCore.Batch.InMemory;
using Zack.EFCore.Batch.Internal;

namespace System.Linq
{
    public static class BatchEFExtensions
    {
        private static IEnumerable<TEntity> BuildItemsToBeRemoved<TEntity>(IQueryable<TEntity> queryable, Expression<Func<TEntity, bool>>? predicate, bool ignoreQueryFilters) where TEntity : class
        {            
            if (ignoreQueryFilters)
            {
                queryable = queryable.IgnoreQueryFilters();
            }
            if (predicate == null)
            {
                return queryable;
            }
            else
            {
                return queryable.Where(predicate);
            }
        }

        public static Task<int> DeleteRangeAsync<TEntity>(this DbContext ctx,
            Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
            where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            var items = BuildItemsToBeRemoved(ctx.Set<TEntity>(), predicate, ignoreQueryFilters);
            ctx.RemoveRange(items);
            return ctx.SaveChangesAsync(cancellationToken);
        }


        public static Task<int> DeleteRangeAsync<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx,
            Expression<Func<TEntity,bool>> predicate=null, bool ignoreQueryFilters = false, CancellationToken cancellationToken = default)
            where TEntity:class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            var items = BuildItemsToBeRemoved(queryable, predicate, ignoreQueryFilters);
            ctx.RemoveRange(items);
            return ctx.SaveChangesAsync(cancellationToken);
        }

        public static int DeleteRange<TEntity>(this DbContext ctx, Expression<Func<TEntity, bool>> predicate=null, bool ignoreQueryFilters = false)
            where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            var items = BuildItemsToBeRemoved(ctx.Set<TEntity>(), predicate, ignoreQueryFilters);
            ctx.RemoveRange(items);
            return ctx.SaveChanges();
        }

        public static int DeleteRange<TEntity>(this IQueryable<TEntity> queryable, DbContext ctx, Expression<Func<TEntity, bool>> predicate = null, bool ignoreQueryFilters = false)
            where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            var items = BuildItemsToBeRemoved(queryable, predicate, ignoreQueryFilters);
            ctx.RemoveRange(items);
            return ctx.SaveChanges();
        }

        public static BatchUpdateBuilder<TEntity> BatchUpdate<TEntity>(this DbContext ctx) where TEntity:class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            DbSet<TEntity> dbSet = ctx.Set<TEntity>();
            BatchUpdateBuilder<TEntity> builder = new BatchUpdateBuilder<TEntity>(ctx, dbSet);
            return builder;
        }

        public static BatchUpdateBuilder<TEntity> BatchUpdate<TEntity>(this DbSet<TEntity> dbSet, DbContext ctx) where TEntity : class
        {
            InMemoryHelper.AssertIsInMemory(ctx);
            BatchUpdateBuilder<TEntity> builder = new BatchUpdateBuilder<TEntity>(ctx, dbSet);
            return builder;
        }
    }
}
