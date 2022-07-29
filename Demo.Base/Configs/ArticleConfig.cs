using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Base.Configs
{
    class ArticleConfig : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> builder)
		{
			builder.ToTable("T_Articles");
			builder.Property(a => a.Content).IsRequired().IsUnicode();
			builder.Property(a => a.Title).IsRequired().IsUnicode().HasMaxLength(255);
			builder.Property(a => a.Id).HasColumnName("PKId");
			builder.OwnsOne(a => a.Remarks).OwnsOne(a => a.Second);
		}
	}

}
