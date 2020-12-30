using System;

namespace Zack.EFCore.Batch.Internal
{
    class EnumHelper
    {
        /// <summary>
        /// 8->Dir.EAST
        /// </summary>
        /// <param name="enumType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Enum FromInt(Type enumType, int value)
        {
            Array values = Enum.GetValues(enumType);
            for (int i = 0; i<values.Length; i++)
            {
                object enumValue = values.GetValue(i);
                int intValue = Convert.ToInt32(enumValue);
                if(intValue == value)
                {
                    return (Enum)enumValue;
                }
            }
            return null;
        }
    }
}
