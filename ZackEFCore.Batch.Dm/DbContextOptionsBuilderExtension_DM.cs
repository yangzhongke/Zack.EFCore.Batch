using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.DM.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_DM
    {
        public static DbContextOptionsBuilder UseBatchEF_DM(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_DM>();
            return optBuilder;
        }
    }
}
