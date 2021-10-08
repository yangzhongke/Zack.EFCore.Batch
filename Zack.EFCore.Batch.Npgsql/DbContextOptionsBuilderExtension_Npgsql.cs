using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.Npgsql.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_Npgsql
    {
        public static DbContextOptionsBuilder UseBatchEF_Npgsql(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Npgsql>();
            return optBuilder;
        }
    }
}
