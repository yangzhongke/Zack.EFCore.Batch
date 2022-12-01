using Microsoft.EntityFrameworkCore.Query;
using Zack.EFCore.Batch_NET7;

namespace Microsoft.EntityFrameworkCore
{
    public static class DbContextOptionsBuilderExtension_Npgsql
    {
        public static DbContextOptionsBuilder UseBatchEF_Npgsql(this DbContextOptionsBuilder optBuilder)
        {
#if (NET7_0_OR_GREATER)
            throw ExceptionHelpers.CreateBatchNotSupportException_InEF7();
#else
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, Zack.EFCore.Batch.Npgsql.Internal.ZackQuerySqlGeneratorFactory_Npgsql>();
            return optBuilder;
#endif
        }
    }
}
