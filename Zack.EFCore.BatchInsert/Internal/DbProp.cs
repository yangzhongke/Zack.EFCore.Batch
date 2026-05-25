using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Zack.EFCore.BatchInsert.Internal;

public class DbProp
{
    public Func<object, object?> GetValueFunc;
    public ValueConverter? ValueConverter { get; set; }

    public string ColumnName { get; set; }

    //public PropertyInfo Property { get; set; }
    public Type PropertyType { get; set; }
}