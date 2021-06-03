using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo
{
    [Table("T_Books", Schema = "MySchema1")]
    public class Book
    {
        public long Id { get; set; }
        public string Title { get; set; }
        public DateTime PubTime { get; set; }
        public double Price { get; set; }
        public string AuthorName { get; set; }
    }
}
