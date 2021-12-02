using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo
{
    [Table("T_Authors")]
    public class Author
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public ICollection<string>? Tags { get; set; }
    }
}
