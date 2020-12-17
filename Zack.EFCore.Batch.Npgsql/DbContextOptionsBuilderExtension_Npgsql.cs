using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.Npgsql.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_Npgsql
    {
        public static void UseBatchEF_Npgsql(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Npgsql>();
        }
    }
}
