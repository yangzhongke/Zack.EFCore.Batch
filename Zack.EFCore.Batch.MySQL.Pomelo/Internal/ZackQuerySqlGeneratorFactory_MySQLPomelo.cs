using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure.Internal;
#if NET5_0
using Pomelo.EntityFrameworkCore.MySql.Query.Internal;
#endif

namespace Zack.EFCore.Batch.MySQL.Pomelo.Internal
{
    class ZackQuerySqlGeneratorFactory_MySQLPomelo : IQuerySqlGeneratorFactory
    {
        private readonly ISqlGenerationHelper _sqlGenerationHelper;
        private readonly QuerySqlGeneratorDependencies _dependencies;
#if NET5_0
        private readonly MySqlSqlExpressionFactory sqlExpressionFactory;
#endif
        private readonly IMySqlOptions _options;

        public ZackQuerySqlGeneratorFactory_MySQLPomelo(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper,
#if NET5_0
            SqlExpressionFactoryDependencies exprFactoryDep,
#endif
            IMySqlOptions options)
        {
            this._dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
#if NET5_0
            this.sqlExpressionFactory = new MySqlSqlExpressionFactory(exprFactoryDep);
#endif
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
