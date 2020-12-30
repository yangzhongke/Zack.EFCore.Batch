using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.Serialization;

namespace Demo.PostgreSQL.Npgsql.heggi
{
    public class CustomEnumConverter<TEnum> : ValueConverter<TEnum, string> where TEnum : struct, Enum
    {
        public CustomEnumConverter(ConverterMappingHints mappingHints = null)
            : base(toStringExpr, toEnumExpr, mappingHints) { }

        private readonly static Expression<Func<TEnum, string>> toStringExpr = x => ToDbString(x);
        private readonly static Expression<Func<string, TEnum>> toEnumExpr = x => ToEnum(x);

        public static string ToDbString(TEnum tEnum)
        {
            var enumType = tEnum.GetType();
            var memInfo = enumType.GetMember(tEnum.ToString());

            var attr = (EnumMemberAttribute)memInfo[0]
                .GetCustomAttributes(typeof(EnumMemberAttribute), false)
                .FirstOrDefault();

            return attr.Value;
        }

        public static TEnum ToEnum(string value)
        {
            foreach (var name in Enum.GetNames<TEnum>())
            {
                var attr = typeof(TEnum)
                    .GetRuntimeField(name)
                    .GetCustomAttribute<EnumMemberAttribute>(true);
                if (attr != null && attr.Value == value)
                {
                    return Enum.Parse<TEnum>(name);
                }
            }

            return default;
        }
    }
}
