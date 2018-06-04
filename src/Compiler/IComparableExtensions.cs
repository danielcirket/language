using System;

namespace Compiler
{
    internal static class IComparableExtensions
    {
        public static T Min<T>(this T source, T min) where T : IComparable<T>
        {
            if (source.CompareTo(min) < 0)
                return min;

            return source;
        }
        public static T Max<T>(this T source, T max) where T : IComparable<T>
        {
            if (source.CompareTo(max) > 0)
                return max;

            return source;
        }
        public static T Clamp<T>(this T source, T min, T max) where T : IComparable<T>
        {
            if (source.CompareTo(min) < 0)
                return min;
            else if (source.CompareTo(max) > 0)
                return max;
            else
                return source;
        }
    }
}
