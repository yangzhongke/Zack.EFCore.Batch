using Demo.Base;
using Microsoft.EntityFrameworkCore;

namespace Demo;

public abstract class BaseDbContext : DbContext
{
    //public DbSet<NodaTimeEntity> NodaTimeEntities { get; set; }
    //private DateTime testDateTime = DateTime.Now;
    public BaseDbContext(DbContextOptions options) : base(options)
    {
    }

    public BaseDbContext()
    {
    }

    public DbSet<Book> Books { get; set; }
    public DbSet<Author> Authors { get; set; }

    public DbSet<Article> Articles { get; set; }
    public DbSet<Comment> Comments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        //modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime>DateTime.Now);
        // modelBuilder.Entity<Book>().HasQueryFilter(b => b.PubTime > DateTime.Now
        //&&b.PubTime> testDateTime);//https://github.com/yangzhongke/Zack.EFCore.Batch/issues/84

        modelBuilder.Entity<Book>().Property("Price").HasDefaultValue(1);
    }
}