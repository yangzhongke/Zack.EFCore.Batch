using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Oracle.EntityFrameworkCore.Infrastructure.Internal;

namespace Zack.EFCore.Batch.Oracle.Internal
{
    class ZackQuerySqlGeneratorFactory_Oracle : IQuerySqlGeneratorFactory
    {
        private ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;

        private readonly IOracleOptions _oracleOptions;

        public ZackQuerySqlGeneratorFactory_Oracle(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, IOracleOptions oracleOptions)
        {
            this._dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
            this._oracleOptions = oracleOptions;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_Oracle(this._dependencies, this._oracleOptions, this._sqlGenerationHelper);
        }
    }
}
