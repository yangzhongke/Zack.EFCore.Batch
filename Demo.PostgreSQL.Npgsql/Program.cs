using Demo.PostgreSQL.Npgsql.heggi;
using System.Linq;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {
        static async Task Main(string[] args)
        {
            using (var ctx = new AppDbContext())
            {
                /*
                User u = new User();
                u.Uid = Guid.NewGuid().ToString();
                u.Status = SessStatus.Stopping;
                db.Add(u);
                db.SaveChanges();*/
                
                ctx.BatchUpdate<User>()
                  .Set(m => m.Status, m => SessStatus.Stopreq) // '1' in DB, must be 'stopreq'
                  .Set(m=>m.Uid,m=>m.Uid+"1")
                  .Where(m=>m.Status== SessStatus.Active)
                  .Execute();
                //var u = ctx.User.Select(u=>new { u.Status, b=SessStatus.Active }).FirstOrDefault();
            }
            /*
            var db = new AppDbContext();
            var ip = IPAddress.Parse("1.0.0.1");

            // This throw Exception
            var user = db.User
                .Where(m => EF.Functions.ContainsOrEqual(m.IPv4.Value, ip))
                .FirstOrDefault();

            db.BatchUpdate<User>().Set(b => b.Uid, b => b.Uid + 3)
                .Where(m => EF.Functions.ContainsOrEqual(m.IPv4.Value, ip))
                .Execute();
            
            using (TestDbContext ctx = new TestDbContext())
            {
                ctx.Books.ToList();
                int n = Convert.ToInt32("3");
                string s = "hello";
                
                await ctx.DeleteRangeAsync<Book>(b => b.Price > n || b.AuthorName == s,true);
                await ctx.BatchUpdate<Book>()
                    .Set(b => b.Price, b => b.Price + 3)
                    .Set(b => b.Title, b => s)
                    .Set(b=>b.AuthorName,b=>b.Title.Substring(3,2))
                    //b.Title.Substring(3,2)+b.AuthorName.ToUpper() is not supported on Npgsql.EntityFrameworkCore.PostgreSQL 5.0.0, so I cannot either.
                    .Set(b => b.PubTime, b => DateTime.Now)
                    .Where(b => b.Id > n || b.AuthorName.StartsWith("Zack"))
                    .ExecuteAsync();

                var b = await ctx.Books.OrderBy(b => b.PubTime).FirstOrDefaultAsync();
                Console.WriteLine(b);
            }*/
        }
    }
}
