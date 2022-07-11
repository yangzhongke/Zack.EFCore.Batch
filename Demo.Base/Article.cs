using System.Collections.Generic;

namespace Demo.Base
{
    public class Article
	{
		public long Id { get; set; }
		public string Title { get; set; }
		public string Content { get; set; }
		public List<Comment> Comments { get; set; } = new List<Comment>();
		public MultiString Remarks { get; set; }
	}

}
