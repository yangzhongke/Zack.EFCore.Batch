using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base.SyncApi
{
    public static class OrderSyncApiDbProperties
    {
        public static string DbTablePrefix { get; set; } = "";

        public static string DbSchema { get; set; } = null;

        public const string ConnectionStringName = "OrderSyncApi";
    }
}
