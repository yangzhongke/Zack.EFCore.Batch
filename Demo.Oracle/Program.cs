using Demo.Base;
using System.Threading.Tasks;
using Oracle.ManagedDataAccess.Client;
using Zack.EFCore.Batch.Oracle;

namespace Demo
{
	class Program
	{

		static async Task Main(string[] args)
		{
			using (TestDbContext ctx = new TestDbContext())
			{
				var list = new List<Book>();

				list.Add(new Book { Title = "1", Price = new Random().Next(0, 100), PubTime = DateTime.Now, AuthorName = "v", Pages = 2 });
				list.Add(new Book { Title = "2", Price = new Random().Next(0, 100), PubTime = DateTime.Now, AuthorName = "v", Pages = 2 });
				list.Add(new Book { Title = "3", Price = new Random().Next(0, 100), PubTime = DateTime.Now, AuthorName = "v", Pages = 2 });
				list.Add(new Book { Title = "4", Price = new Random().Next(0, 100), PubTime = DateTime.Now, AuthorName = "v", Pages = 2 });

				await ctx.BulkInsertAsync(list, OracleBulkCopyOptions.UseInternalTransaction);
			}
		}
	}
}
