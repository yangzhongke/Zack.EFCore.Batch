using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace Zack.EFCore.Batch.Internal;

internal static class BulkInsertExecutorResolver
{
    private static readonly object SyncRoot = new();

    public static IBulkInsertExecutor? Resolve(DbContext dbCtx)
    {
        lock (SyncRoot)
        {
            var executors = GetExecutors();
            foreach (var executor in executors)
                if (executor.CanHandle(dbCtx))
                    return executor;

            return null;
        }
    }

    private static List<IBulkInsertExecutor> GetExecutors()
    {
        var result = new List<IBulkInsertExecutor>();
        var interfaceType = typeof(IBulkInsertExecutor);
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic) continue;

            Type[] types;
            try
            {
                types = assembly.GetTypes();
            }
            catch (ReflectionTypeLoadException ex)
            {
                types = ex.Types.Where(t => t != null).Cast<Type>().ToArray();
            }

            foreach (var type in types)
            {
                if (!interfaceType.IsAssignableFrom(type) || type.IsInterface || type.IsAbstract) continue;

                var ctor = type.GetConstructor(Type.EmptyTypes);
                if (ctor == null) continue;

                if (Activator.CreateInstance(type) is IBulkInsertExecutor executor) result.Add(executor);
            }
        }

        return result;
    }
}