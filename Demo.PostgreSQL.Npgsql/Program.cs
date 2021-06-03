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
                  //.Set(m=>m.Status,m=>m.Uid=="a"?SessStatus.Stopreq:SessStatus.Stopping) //not supported. Only assignment of constant values to enumerated types is supported currently.
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
                .Execute();*/
        }
    }
}
