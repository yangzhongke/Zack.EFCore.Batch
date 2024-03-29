﻿using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Demo.Base.Configs
{
    class CommentConfig : IEntityTypeConfiguration<Comment>
	{
		public void Configure(EntityTypeBuilder<Comment> builder)
		{
			builder.ToTable("T_Comments");
			builder.HasOne<Article>(c => c.Article).WithMany(a => a.Comments).IsRequired();
			builder.Property(c => c.Message).IsRequired().IsUnicode();
			builder.Property(c => c.Id).HasColumnName("PKId");
		}
	}

}
