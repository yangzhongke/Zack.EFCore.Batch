using System.ComponentModel.DataAnnotations.Schema;

namespace Demo
{
    [Table("T_Authors")]
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
    }
}
