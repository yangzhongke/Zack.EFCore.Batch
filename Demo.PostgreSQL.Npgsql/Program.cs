using Demo.PostgreSQL.Npgsql.heggi;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace Demo
{
    class Program
    {

        static async Task Main(string[] args)
        {
            
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
            }
        }
    }
}
