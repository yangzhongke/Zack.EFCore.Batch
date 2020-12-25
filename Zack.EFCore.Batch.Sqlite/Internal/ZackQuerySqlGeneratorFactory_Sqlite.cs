using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Zack.EFCore.Batch.Sqlite.Internal
{
    class ZackQuerySqlGeneratorFactory_Sqlite : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;
        private ISqlGenerationHelper _sqlGenerationHelper;

        public ZackQuerySqlGeneratorFactory_Sqlite(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_Sqlite(this.dependencies, this._sqlGenerationHelper);
        }
    }
}
