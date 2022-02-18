using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Base.Configs
{
    class CommentConfig : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.HasOne<Article>(c => c.Article).WithMany(a => a.Comments).IsRequired();
			builder.Property(c => c.Message).IsRequired().IsUnicode();
		}
	}

}
