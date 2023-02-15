using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
#if (!NET7_0_OR_GREATER)
using Zack.EFCore.Batch.Oracle.Internal;
#endif
using Zack.EFCore.Batch_NET7;

namespace Zack.EFCore.Batch.Oracle
{
    public static class DbContextOptionsBuilderExtension_Oracle
    {
        public static DbContextOptionsBuilder UseBatchEF_Oracle(this DbContextOptionsBuilder optBuilder)
        {
#if (NET7_0_OR_GREATER)
            throw ExceptionHelpers.CreateBatchNotSupportException_InEF7();
#else
            optBuilder.ReplaceService<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Oracle>();
            return optBuilder;
#endif
        }
    }
}
