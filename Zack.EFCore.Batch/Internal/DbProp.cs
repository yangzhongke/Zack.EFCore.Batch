using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Reflection;

namespace Zack.EFCore.Batch.Internal
{
    public class DbProp
    {
        public ValueConverter? ValueConverter { get; set; }
        public string ColumnName { get; set; }

        //public Func<object, object?> GetValueFunc;
        //public PropertyInfo Property { get; set; }

        public List<PropertyInfo> Properties { get; set; }
        //public Type PropertyType { get; set; }
    }
}
