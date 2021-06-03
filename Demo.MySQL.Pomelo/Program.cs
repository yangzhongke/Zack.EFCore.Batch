using Demo.Base;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            using (TestDbContext ctx = new TestDbContext())
            {
                await TestCase1.RunAsync(ctx);
            }
        }
    }
}
