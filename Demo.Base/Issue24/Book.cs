using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Base.Issue24
{
    [Table("T_Books")]
    public class Book : BaseEntity
    {
        public Guid? Title { get; set; }
        public DateTime? PubTime { get; set; }
        public double Price { get; set; }
        public string AuthorName { get; set; }
        public int? Pages { get; set; }
        /// <summary>
        /// 访视是否激活可见
        /// </summary>
        public bool? Available { get; set; }
    }
}
