using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Zack.EFCore.Batch.Internal
{
    internal static class BulkInsertExecutorResolver
    {
        private const string EmptyProviderCacheKey = "<EMPTY_PROVIDER_NAME>";
        private static readonly object SyncRoot = new object();
        private static readonly Dictionary<string, IBulkInsertExecutor?> ProviderCache = new();
        private static List<IBulkInsertExecutor>? _executors;

        static BulkInsertExecutorResolver()
        {
            AppDomain.CurrentDomain.AssemblyLoad += (_, _) =>
            {
                lock (SyncRoot)
                {
                    _executors = null;
                    ProviderCache.Clear();
                }
            };
        }

        public static IBulkInsertExecutor? Resolve(DbContext dbCtx)
        {
            var providerCacheKey = GetProviderCacheKey(dbCtx.Database.ProviderName);
            lock (SyncRoot)
            {
                if (ProviderCache.TryGetValue(providerCacheKey, out var cachedExecutor))
                {
                    return cachedExecutor;
                }

                var executors = GetExecutorsUnsafe();
                foreach (var executor in executors)
                {
                    if (executor.CanHandle(dbCtx))
                    {
                        ProviderCache[providerCacheKey] = executor;
                        return executor;
                    }
                }

                ProviderCache[providerCacheKey] = null;
                return null;
            }
        }

        private static string GetProviderCacheKey(string? providerName)
        {
            return string.IsNullOrEmpty(providerName) ? EmptyProviderCacheKey : providerName;
        }

        private static List<IBulkInsertExecutor> GetExecutorsUnsafe()
        {
            if (_executors != null)
            {
                return _executors;
            }

            var result = new List<IBulkInsertExecutor>();
            var interfaceType = typeof(IBulkInsertExecutor);
            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (assembly.IsDynamic)
                {
                    continue;
                }

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
                    if (!interfaceType.IsAssignableFrom(type) || type.IsInterface || type.IsAbstract)
                    {
                        continue;
                    }

                    var ctor = type.GetConstructor(Type.EmptyTypes);
                    if (ctor == null)
                    {
                        continue;
                    }

                    if (Activator.CreateInstance(type) is IBulkInsertExecutor executor)
                    {
                        result.Add(executor);
                    }
                }
            }

            _executors = result;
            return result;
        }

        internal static void ClearCache()
        {
            lock (SyncRoot)
            {
                ProviderCache.Clear();
            }
        }
    }
}


