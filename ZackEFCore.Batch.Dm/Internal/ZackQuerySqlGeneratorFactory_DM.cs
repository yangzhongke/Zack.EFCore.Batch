using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Zack.EFCore.Batch.DM.Internal
{
    class ZackQuerySqlGeneratorFactory_DM : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;

        private ISqlGenerationHelper _sqlGenerationHelper;

        public ZackQuerySqlGeneratorFactory_DM(QuerySqlGeneratorDependencies dependencies, ISqlGenerationHelper sqlGenerationHelper)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_DM(this.dependencies, this._sqlGenerationHelper);
        }
    }
}
