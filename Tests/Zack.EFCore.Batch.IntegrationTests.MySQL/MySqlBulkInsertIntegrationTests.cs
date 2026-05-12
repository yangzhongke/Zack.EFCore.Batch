using Demo;
using Demo.Base;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;
using Xunit;
using Zack.EFCore.Batch.IntegrationTests.Shared;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Zack.EFCore.Batch.IntegrationTests.MySql;

public class MySqlBulkInsertIntegrationTests
{
    private const string ConnectionStringEnvName = "TEST_DB_MYSQL_CS";
    private const string FallbackLogPrefix = "No IBulkInsertExecutor matched provider";

    [Fact]
    public async Task BulkInsertAndBulkInsertAsync_InsertExpectedRows()
    {
        var connStr = Environment.GetEnvironmentVariable(ConnectionStringEnvName);
        if (string.IsNullOrWhiteSpace(connStr)) return;

        using var fallbackLogSink = new FallbackLogSink();
        using var loggerFactory = LoggerFactory.Create(builder =>
        {
            builder.SetMinimumLevel(LogLevel.Information);
            builder.AddProvider(fallbackLogSink);
        });

        await using var dbCtx = new MySqlIntegrationDbContext(connStr, loggerFactory);
        await dbCtx.Database.EnsureCreatedAsync();
        dbCtx.Comments.RemoveRange(dbCtx.Comments);
        dbCtx.Articles.RemoveRange(dbCtx.Articles);
        dbCtx.Books.RemoveRange(dbCtx.Books);
        dbCtx.Authors.RemoveRange(dbCtx.Authors);
        await dbCtx.SaveChangesAsync();

        var authors = TestBulkInsert1.BuildAuthors().Take(20).ToList();
        dbCtx.BulkInsert(authors);
        Assert.All(authors, a => Assert.Equal(EntityState.Unchanged, dbCtx.Entry(a).State));

        var books = TestBulkInsert1.BuildBooks().Take(20).ToList();
        await dbCtx.BulkInsertAsync(books);
        Assert.All(books, b => Assert.Equal(EntityState.Unchanged, dbCtx.Entry(b).State));

        var articles = TestOwnedType.BuildArticlesForInsert(8).ToList();
        await dbCtx.BulkInsertAsync(articles);
        Assert.All(articles, a => Assert.Equal(EntityState.Unchanged, dbCtx.Entry(a).State));

        Assert.Equal(20, await dbCtx.Authors.CountAsync());
        Assert.Equal(20, await dbCtx.Books.CountAsync());
        Assert.Equal(8, await dbCtx.Articles.CountAsync());
        Assert.True(await dbCtx.Books.AnyAsync(b => b.BookType == BookType.Fictional));
        Assert.True(await dbCtx.Articles.AnyAsync(a =>
            a.Remarks != null && a.Remarks.Chinese != null && a.Remarks.Chinese.Contains("中国人")));
        Assert.DoesNotContain(fallbackLogSink.Messages,
            m => m.Contains(FallbackLogPrefix, StringComparison.Ordinal));
    }

    private sealed class MySqlIntegrationDbContext : BaseDbContext
    {
        private readonly string _connStr;
        private readonly ILoggerFactory _loggerFactory;

        public MySqlIntegrationDbContext(string connStr, ILoggerFactory loggerFactory)
        {
            _connStr = connStr;
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySql(_connStr, ServerVersion.AutoDetect(_connStr),
                builder => { builder.SchemaBehavior(MySqlSchemaBehavior.Ignore); });
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }

}


