using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Zack.EFCore.Batch.MySQL.Pomelo.Internal
{
    class ZackQuerySqlGeneratorFactory_MySQLPomelo : IQuerySqlGeneratorFactory
    {
        private ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly MySqlSqlExpressionFactory _sqlExpressionFactory;

        private readonly IMySqlOptions _options;

        public ZackQuerySqlGeneratorFactory_MySQLPomelo(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, ISqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
        {
            this._dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
            this._sqlExpressionFactory = (MySqlSqlExpressionFactory)sqlExpressionFactory;
            this._options = options;
        }
        public QuerySqlGenerator Create()
        {
            return new ZackQuerySqlGenerator_MySQLPomelo(this._dependencies,this._sqlGenerationHelper, _sqlExpressionFactory, _options);
        }
    }
}
