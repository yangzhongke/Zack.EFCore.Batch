using Microsoft.EntityFrameworkCore;
using System;

namespace Zack.EFCore.Batch.InMemory
{
    internal static class InMemoryHelper
    {
        public static void AssertIsInMemory(this DbContext ctx)
        {
            if(!ctx.Database.IsInMemory())
            {
                throw new InvalidOperationException("only supported on InMemory");
            }
        }
    }
}
