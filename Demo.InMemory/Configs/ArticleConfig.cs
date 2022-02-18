using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Base.Configs
{
    class ArticleConfig : IEntityTypeConfiguration<Article>
	{
		public void Configure(EntityTypeBuilder<Article> builder)
		{
			builder.Property(a => a.Content).IsRequired().IsUnicode();
			builder.Property(a => a.Title).IsRequired().IsUnicode().HasMaxLength(255);
		}
	}

}
