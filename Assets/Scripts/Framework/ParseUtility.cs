using System.ComponentModel;

namespace Framework
{
    public sealed class ParseUtility
    {
        public static bool TryParse<From, To> (From from, ref To to)
        {
            try
            {
                TypeConverter converter = TypeDescriptor.GetConverter (typeof (To));
                if (converter != null)
                {
                    to = (To)converter.ConvertFrom (from);
                    return true;
                }
                return false;
            }
            catch
            {
                return false;
            }
        }

        public static bool Is<From, To> ()
        {
            TypeConverter converter = TypeDescriptor.GetConverter (typeof(From));
            return converter.CanConvertTo (typeof(To));
        }
    }
}