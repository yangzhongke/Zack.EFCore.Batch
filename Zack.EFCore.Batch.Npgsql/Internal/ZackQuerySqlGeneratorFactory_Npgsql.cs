using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal;

namespace Zack.EFCore.Batch.Npgsql.Internal
{
    class ZackQuerySqlGeneratorFactory_Npgsql : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;
        private ISqlGenerationHelper _sqlGenerationHelper;
        readonly INpgsqlOptions _npgsqlOptions;

        public ZackQuerySqlGeneratorFactory_Npgsql(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, INpgsqlOptions npgsqlOptions)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
            this._npgsqlOptions = npgsqlOptions;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_Npgsql(this.dependencies, this._sqlGenerationHelper,
                _npgsqlOptions.ReverseNullOrderingEnabled,
                _npgsqlOptions.PostgresVersion);
        }
    }
}
