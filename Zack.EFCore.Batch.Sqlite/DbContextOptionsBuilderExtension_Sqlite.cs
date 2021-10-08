using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.Sqlite.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_Sqlite
    {
        public static DbContextOptionsBuilder UseBatchEF_Sqlite(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Sqlite>();
            return optBuilder;
        }
    }
}
