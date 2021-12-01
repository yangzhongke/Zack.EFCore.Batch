using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.MSSQL.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_MSSQL
    {
        public static DbContextOptionsBuilder UseBatchEF_MSSQL(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_MSSQL>();
            return optBuilder;
        }
    }
}
