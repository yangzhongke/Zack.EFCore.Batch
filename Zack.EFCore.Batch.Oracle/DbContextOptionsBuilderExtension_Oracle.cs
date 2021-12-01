using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch.Oracle.Internal;

namespace Zack.EFCore.Batch.Oracle
{
    public static class DbContextOptionsBuilderExtension_Oracle
    {
        public static DbContextOptionsBuilder UseBatchEF_Oracle(this DbContextOptionsBuilder optBuilder)
        {
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Oracle>();
            return optBuilder;
        }
    }
}
