using NodaTime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Base
{
    public class NodaTimeEntity
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public Instant Instant { get; set; }
    }
}
