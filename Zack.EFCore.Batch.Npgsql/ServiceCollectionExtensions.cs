using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Zack.EFCore.Batch.Npgsql.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBatchEF_Npgsql(this IServiceCollection services)
        {
            return services.AddScoped<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_Npgsql>();
        }
    }
}
