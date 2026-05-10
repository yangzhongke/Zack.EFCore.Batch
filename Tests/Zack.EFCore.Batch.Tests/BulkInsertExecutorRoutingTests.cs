using System.Collections;
using Microsoft.EntityFrameworkCore;
using Xunit;
using Zack.EFCore.Batch.Internal;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Zack.EFCore.Batch.Tests;

public class BulkInsertExecutorRoutingTests
{
    [Fact]
    public async Task MatchedExecutor_IsCachedByProviderName()
    {
        FakeTestExecutor.Reset();
        FakeTestExecutor.MatchInMemoryProvider = true;
        BulkInsertExecutorResolver.ClearCache();

        await using var dbCtx = new RoutingTestDbContext();

        await dbCtx.BulkInsertAsync(new[] { new RoutingEntity { Name = "A" } });
        await dbCtx.BulkInsertAsync(new[] { new RoutingEntity { Name = "B" } });

        Assert.Equal(2, FakeTestExecutor.AsyncCallCount);
        Assert.Equal(1, FakeTestExecutor.CanHandleCallCount);
        Assert.Equal(2, await dbCtx.Entities.CountAsync());
    }

    [Fact]
    public async Task SameProviderAcrossDifferentContexts_HitsProviderCache()
    {
        FakeTestExecutor.Reset();
        FakeTestExecutor.MatchInMemoryProvider = true;
        BulkInsertExecutorResolver.ClearCache();

        await using (var dbCtx1 = new RoutingTestDbContext())
        {
            await dbCtx1.BulkInsertAsync(new[] { new RoutingEntity { Name = "A" } });
        }

        await using (var dbCtx2 = new RoutingTestDbContext())
        {
            await dbCtx2.BulkInsertAsync(new[] { new RoutingEntity { Name = "B" } });
        }

        Assert.Equal(2, FakeTestExecutor.AsyncCallCount);
        Assert.Equal(1, FakeTestExecutor.CanHandleCallCount);
    }

    [Fact]
    public async Task NoMatchedExecutor_UsesFallbackPath()
    {
        FakeTestExecutor.Reset();
        FakeTestExecutor.MatchInMemoryProvider = false;
        BulkInsertExecutorResolver.ClearCache();

        await using var dbCtx = new RoutingTestDbContext();

        await dbCtx.BulkInsertAsync(new[] { new RoutingEntity { Name = "A" } });
        await dbCtx.BulkInsertAsync(new[] { new RoutingEntity { Name = "B" } });

        Assert.Equal(0, FakeTestExecutor.AsyncCallCount);
        Assert.Equal(1, FakeTestExecutor.CanHandleCallCount);
        Assert.Equal(2, await dbCtx.Entities.CountAsync());
    }

    private sealed class RoutingTestDbContext : DbContext
    {
        public DbSet<RoutingEntity> Entities => Set<RoutingEntity>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(Guid.NewGuid().ToString());
        }
    }

    private sealed class RoutingEntity
    {
        public int Id { get; set; }

        public string? Name { get; set; }
    }

    public sealed class FakeTestExecutor : IBulkInsertExecutor
    {
        private const string InMemoryProviderName = "Microsoft.EntityFrameworkCore.InMemory";

        public static int CanHandleCallCount { get; private set; }

        public static int AsyncCallCount { get; private set; }

        public static bool MatchInMemoryProvider { get; set; }

        public bool CanHandle(DbContext dbCtx)
        {
            CanHandleCallCount++;
            return MatchInMemoryProvider && dbCtx.Database.ProviderName == InMemoryProviderName;
        }

        public async Task BulkInsertAsync(DbContext dbCtx, Type entityType, IEnumerable items,
            CancellationToken cancellationToken = default)
        {
            AsyncCallCount++;
            dbCtx.AddRange(items.Cast<object>());
            await dbCtx.SaveChangesAsync(cancellationToken);
        }

        public void BulkInsert(DbContext dbCtx, Type entityType, IEnumerable items)
        {
            dbCtx.AddRange(items.Cast<object>());
            dbCtx.SaveChanges();
        }

        public static void Reset()
        {
            CanHandleCallCount = 0;
            AsyncCallCount = 0;
            MatchInMemoryProvider = false;
        }
    }
}