using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.DependencyInjection;
using Zack.EFCore.Batch.DM.Internal;

namespace Microsoft.EntityFrameworkCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddBatchEF_DM(this IServiceCollection services)
        {
            return services.AddScoped<IQuerySqlGeneratorFactory, ZackQuerySqlGeneratorFactory_DM>();
        }
    }
}
