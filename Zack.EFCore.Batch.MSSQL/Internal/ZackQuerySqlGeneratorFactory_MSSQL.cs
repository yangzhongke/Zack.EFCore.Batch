#if (!NET7_0_OR_GREATER)
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Zack.EFCore.Batch.MSSQL.Internal
{
    class ZackQuerySqlGeneratorFactory_MSSQL : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;

        private ISqlGenerationHelper _sqlGenerationHelper;

        public ZackQuerySqlGeneratorFactory_MSSQL(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_MSSQL(this.dependencies, this._sqlGenerationHelper);
        }
    }
}
#endif