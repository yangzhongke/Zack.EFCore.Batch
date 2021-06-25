using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.EntityFrameworkCore.Modeling;

namespace Demo.Base.SyncApi
{
    public class OrderSyncApiModelBuilderConfigurationOptions : AbpModelBuilderConfigurationOptions
    {
        public OrderSyncApiModelBuilderConfigurationOptions(
            string tablePrefix = "",
            string schema = null)
            : base(
                tablePrefix,
                schema)
        {

        }
    }
}
