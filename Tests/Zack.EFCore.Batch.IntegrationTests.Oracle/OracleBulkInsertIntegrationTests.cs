using Demo;
using Demo.Base;
using Microsoft.EntityFrameworkCore;
using Xunit;

[assembly: CollectionBehavior(DisableTestParallelization = true)]

namespace Zack.EFCore.Batch.IntegrationTests.Oracle;

public class OracleBulkInsertIntegrationTests
{
    private const string ConnectionStringEnvName = "TEST_DB_ORACLE_CS";

    [Fact]
    public async Task BulkInsertAndBulkInsertAsync_InsertExpectedRows()
    {
        var connStr = Environment.GetEnvironmentVariable(ConnectionStringEnvName);
        if (string.IsNullOrWhiteSpace(connStr)) return;

        await using var dbCtx = new OracleIntegrationDbContext(connStr);
        await dbCtx.Database.EnsureCreatedAsync();
        dbCtx.Comments.RemoveRange(dbCtx.Comments);
        dbCtx.Articles.RemoveRange(dbCtx.Articles);
        dbCtx.Books.RemoveRange(dbCtx.Books);
        dbCtx.Authors.RemoveRange(dbCtx.Authors);
        await dbCtx.SaveChangesAsync();

        var authors = TestBulkInsert1.BuildAuthors().Take(20).ToList();
        dbCtx.BulkInsert(authors);

        var books = TestBulkInsert1.BuildBooks().Take(20).ToList();
        await dbCtx.BulkInsertAsync(books);

        var articles = TestOwnedType.BuildArticlesForInsert(8).ToList();
        await dbCtx.BulkInsertAsync(articles);

        Assert.Equal(20, await dbCtx.Authors.CountAsync());
        Assert.Equal(20, await dbCtx.Books.CountAsync());
        Assert.Equal(8, await dbCtx.Articles.CountAsync());
        Assert.True(await dbCtx.Books.AnyAsync(b => b.BookType == BookType.Fictional));
        Assert.True(await dbCtx.Articles.AnyAsync(a =>
            a.Remarks != null && a.Remarks.Chinese != null && a.Remarks.Chinese.Contains("中国人")));
    }

    private sealed class OracleIntegrationDbContext : BaseDbContext
    {
        private readonly string _connStr;

        public OracleIntegrationDbContext(string connStr)
        {
            _connStr = connStr;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseOracle(_connStr);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Book>().ToTable("T_Books");
        }
    }
}


