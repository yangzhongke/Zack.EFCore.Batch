using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text;

namespace Demo.PostgreSQL.Npgsql.heggi
{
    [Table("user")]
    public class User
    {
        [Key, Column("uid")]
        public string Uid { get; set; }

        [Column("ip")]
        public (IPAddress, int)? IPv4 { get; set; }
    }
}
