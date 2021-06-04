using Microsoft.EntityFrameworkCore.Metadata;
using System;
using System.Reflection;

namespace Zack.EFCore.Batch.Internal
{
    public class DbProp
    {
        public IProperty EFMeta { get; set; }
        public string ColumnName { get; set; }
        public string ColumnType { get; set; }

        public PropertyInfo Property { get; set; }
        public string PropertyName { get; set; }
        public Type PropertyType { get; set; }
    }
}
