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

				list.Add(new Book { Title = "123" });

				await ctx.BulkInsertAsync(list, OracleBulkCopyOptions.UseInternalTransaction);
			}
		}
	}
}
