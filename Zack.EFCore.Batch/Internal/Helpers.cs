namespace System
{
    internal static class Helpers
    {
        public static bool IsNullableType(this Type type)
        {
            return Nullable.GetUnderlyingType(type) != null;
        }
    }
}
