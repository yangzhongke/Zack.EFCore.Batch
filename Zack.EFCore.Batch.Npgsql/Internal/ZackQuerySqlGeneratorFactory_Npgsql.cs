using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure;

namespace Zack.EFCore.Batch.Npgsql.Internal
{
    class ZackQuerySqlGeneratorFactory_Npgsql : IQuerySqlGeneratorFactory
    {
        private QuerySqlGeneratorDependencies dependencies;
        private ISqlGenerationHelper _sqlGenerationHelper;
        //readonly INpgsqlOptions _npgsqlOptions;
        private readonly object objOptions;

        public ZackQuerySqlGeneratorFactory_Npgsql(QuerySqlGeneratorDependencies dependencies,
            ISqlGenerationHelper sqlGenerationHelper,IServiceProvider sp)//, INpgsqlOptions npgsqlOptions)
        {
            this.dependencies = dependencies;
            this._sqlGenerationHelper = sqlGenerationHelper;
            //this._npgsqlOptions = npgsqlOptions;

            //fix: https://github.com/yangzhongke/Zack.EFCore.Batch/issues/75
            var asmPostgreSQL = typeof(NpgsqlDbContextOptionsBuilder).Assembly;
            Type? typeOptions;
            typeOptions = asmPostgreSQL.GetType("Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.INpgsqlOptions");
            if(typeOptions == null)//version of Npgsql.EntityFrameworkCore.PostgreSQL is equal or greater than 6.0.5
                                   //INpgsqlOptions has been renamed to INpgsqlSingletonOptions
            {
                typeOptions = asmPostgreSQL.GetType("Npgsql.EntityFrameworkCore.PostgreSQL.Infrastructure.Internal.INpgsqlSingletonOptions");
            }
            if(typeOptions==null)
            {
                throw new ApplicationException("Cannot find INpgsqlOptions nor INpgsqlSingletonOptions");
            }
            objOptions = sp.GetService(typeOptions);
            if(objOptions==null)
            {
                throw new ApplicationException("Cannot find an instance of "+typeOptions);
            }
        }
        public QuerySqlGenerator Create()
        {
            object? reverseNullOrderingEnabled = objOptions.GetType().GetProperty("ReverseNullOrderingEnabled")?.GetValue(objOptions);
            object? postgresVersion = objOptions.GetType().GetProperty("PostgresVersion")?.GetValue(objOptions);
            if(reverseNullOrderingEnabled==null)
            {
                throw new ApplicationException("Cannot find property of ReverseNullOrderingEnabled");
            }
            if (postgresVersion == null)
            {
                throw new ApplicationException("Cannot find property of PostgresVersion");
            }

            /*
            return new ZackQuerySqlGenerator_Npgsql(this.dependencies, this._sqlGenerationHelper,
                _npgsqlOptions.ReverseNullOrderingEnabled,
                _npgsqlOptions.PostgresVersion);*/
            return new ZackQuerySqlGenerator_Npgsql(this.dependencies, this._sqlGenerationHelper,
                (bool)reverseNullOrderingEnabled,
                (Version)postgresVersion);
        }
    }
}
