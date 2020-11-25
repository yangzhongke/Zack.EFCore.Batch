using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Zack.EFCore.Batch
{
    public class ZackQuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;
        private ISqlGenerationHelper _sqlGenerationHelper;
        public ZackQuerySqlGeneratorFactory(QuerySqlGeneratorDependencies dependencies, 
            ISqlGenerationHelper sqlGenerationHelper)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator(this.dependencies, this._sqlGenerationHelper);
        }
    }
}
