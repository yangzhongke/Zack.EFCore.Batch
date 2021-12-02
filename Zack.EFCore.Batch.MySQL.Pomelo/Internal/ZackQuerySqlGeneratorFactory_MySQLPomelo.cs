using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;

namespace Zack.EFCore.Batch.MySQL.Pomelo.Internal
{
    class ZackQuerySqlGeneratorFactory_MySQLPomelo : IQuerySqlGeneratorFactory
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;
        private readonly MySqlSqlExpressionFactory sqlExpressionFactory;

        private readonly IMySqlOptions _options;

        public ZackQuerySqlGeneratorFactory_MySQLPomelo(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper, MySqlSqlExpressionFactory sqlExpressionFactory, IMySqlOptions options)
        {
            this._dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
            this.sqlExpressionFactory = sqlExpressionFactory;
            this._options = options;
        }
        public QuerySqlGenerator Create()
        {
#if NET5_0
            return new ZackQuerySqlGenerator_MySQLPomelo(this._dependencies, this._sqlGenerationHelper, 
                sqlExpressionFactory, _options);

#else
            return new ZackQuerySqlGenerator_MySQLPomelo(this._dependencies,
            this._sqlGenerationHelper, _options);	
#endif
        }
    }
}
