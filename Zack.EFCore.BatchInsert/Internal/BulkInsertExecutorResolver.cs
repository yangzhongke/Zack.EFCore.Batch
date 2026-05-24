using System.Reflection;
using System.Runtime.Loader;
using Microsoft.EntityFrameworkCore;

namespace Zack.EFCore.BatchInsert.Internal;

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
        EnsureProviderAssembliesLoaded();

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

    private static void EnsureProviderAssembliesLoaded()
    {
        var baseDirectory = AppContext.BaseDirectory;
        if (string.IsNullOrWhiteSpace(baseDirectory) || !Directory.Exists(baseDirectory)) return;

        var loadedAssemblyNames = new HashSet<string>(
            AppDomain.CurrentDomain.GetAssemblies()
                .Select(a => a.GetName().Name)
                .Where(n => !string.IsNullOrWhiteSpace(n))!,
            StringComparer.OrdinalIgnoreCase);

        foreach (var assemblyPath in Directory.EnumerateFiles(baseDirectory, "*.dll", SearchOption.TopDirectoryOnly))
        {
            var assemblyName = Path.GetFileNameWithoutExtension(assemblyPath);
            if (!IsProviderAssemblyName(assemblyName) || loadedAssemblyNames.Contains(assemblyName)) continue;

            try
            {
                AssemblyLoadContext.Default.LoadFromAssemblyPath(assemblyPath);
            }
            catch
            {
                // Ignore load failures and continue scanning already loaded assemblies.
            }
        }
    }

    private static bool IsProviderAssemblyName(string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return false;

        return name.StartsWith("Zack.EFCore.BatchInsert", StringComparison.Ordinal);
    }
}