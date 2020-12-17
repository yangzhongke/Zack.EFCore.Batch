using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Diagnostics.Internal;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Zack.EFCore.Batch.Internal
{
    static class  LogHelper
    {
        public static void Log(this DbContext dbContext, string msg)
        {
            var logger = dbContext.GetService<IDiagnosticsLogger<DbLoggerCategory.Update>>();
            EventDefinitionBase eventData = CoreResources.LogQueryExecutionPlanned(logger);
            logger.DbContextLogger.Log(new EventData(eventData, (s1, s2) => msg));
        }
    }
}
