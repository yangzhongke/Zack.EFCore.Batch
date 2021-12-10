using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Zack.EFCore.Batch.MySQL.Pomelo.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBatchEF_MySQLPomelo(this IServiceCollection services)
        {
            return services.AddScoped<IQuerySqlGeneratorFactory,ZackQuerySqlGeneratorFactory_MySQLPomelo>();
        }
    }
}
