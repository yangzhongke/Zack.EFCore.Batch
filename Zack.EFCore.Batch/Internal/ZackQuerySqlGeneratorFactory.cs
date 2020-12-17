using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;

namespace Zack.EFCore.Batch.Internal
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
            if(_sqlGenerationHelper.GetType().Name.Contains("Npgsql"))
            {
                throw new System.Exception("If Npgsql.EntityFrameworkCore.PostgreSQL is used, please use the Postgresql-specific NuGet Package instead, called 'Zack.EFCore.Batch.Npgsql' https://www.nuget.org/packages/Zack.EFCore.Batch.Npgsql");
            }
            return new ZackQuerySqlGenerator(this.dependencies, this._sqlGenerationHelper);
        }
    }
}
