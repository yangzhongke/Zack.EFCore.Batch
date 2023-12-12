using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo
{
    [Table("T_Books", Schema = "MySchema1")]
    public class Book
    {
        [Column("Id")]
        public long Id { get; set; }

        [Column("Title")]
		public string? Title { get; set; }

        [Column("PubTime")]
        public DateTime? PubTime { get; set; }

        [Column("Price")]
		public int Price { get; set; }

		[Column("AuthorName")]
		public string? AuthorName { get; set; }

		[Column("RO1")]
		public string RO1 { get { return "xxxx"; } }
        [NotMapped]
        public string NotMappF { get; set; }

        [Column("Pages")]
		public int Pages { get; set; }

        public BookType BookType { get; set; }
	}
}
