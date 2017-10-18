using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UAM.InformatiX
{
    internal static class ReflectionExtensions
    {
        public static bool IsDefined<TAttribute>(this MemberInfo memberInfo, bool inherit)
        {
            return memberInfo.IsDefined(typeof(TAttribute), inherit);
        }
        public static bool Implements<TInterface>(this Type type)
        {
            return Array.IndexOf(type.GetInterfaces(), typeof(TInterface)) >= 0;
        }
        public static bool Implements(this Type type, Type interfaceType, out Type[] matchingTypes)
        {
            Type[] implementedTypes = type.GetInterfaces();

            if (interfaceType.IsGenericTypeDefinition)
            {
                List<Type> closedTypes = new List<Type>(1); // we expect single instance in most cases

                foreach (Type implementedType in implementedTypes)
                    if (implementedType.IsGenericType)
                        if (implementedType.GetGenericTypeDefinition() == interfaceType)
                            closedTypes.Add(implementedType);

                matchingTypes = closedTypes.ToArray();
                return closedTypes.Count > 0;
            }
            else
            {
                matchingTypes = null;

                foreach (Type implementedType in implementedTypes)
                    if (implementedType == interfaceType)
                    {
                        matchingTypes = new Type[] { implementedType };
                        return true;
                    }

                matchingTypes = new Type[0];
                return false;
            }
        }

        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this MemberInfo memberInfo, bool inherit)
        {
            return memberInfo.GetCustomAttributes(typeof(TAttribute), inherit).Cast<TAttribute>();
        }
        public static IEnumerable<TAttribute> GetCustomAttributes<TAttribute>(this Assembly assembly)
        {
            return assembly.GetCustomAttributes(typeof(TAttribute), false).Cast<TAttribute>();
        }

        public static object GetDefault(this Type type)
        {
            if (!type.IsValueType || Nullable.GetUnderlyingType(type) != null)
                return null;

            Func<object> func = GetDefault<object>;
            return func.Method.GetGenericMethodDefinition().MakeGenericMethod(type).Invoke(null, null);

        }
        private static T GetDefault<T>()
        {
            return default(T);
        }
    }
    
    internal static class StringExtensions
    {
        public static string ToCamelCaseInvariant(this string s)
        {
            if (string.IsNullOrEmpty(s) || char.IsLower(s, 0))
                return s;

            if (s.Length == 1)
                return char.ToLowerInvariant(s[0]).ToString();

            return char.ToLowerInvariant(s[0]).ToString() + s.Substring(1);
        }

        internal static bool IsNullOrWhiteSpace(string value)
        {
            if (value == null) 
                return true;

            for (int i = 0; i < value.Length; i++)
                if (!char.IsWhiteSpace(value[i])) return false;

            return true;
        }
    }

    internal static class BinaryExtensions
    {
        public static int ReadBigInt32(this Stream stream)
        {
            byte[] data = new byte[4];
            if (stream.Read(data, 0, 4) < 4)
                throw new EndOfStreamException();

            return
                data[0] << 24 |
                data[1] << 16 |
                data[2] << 8 |
                data[3];
        }

        public static void WriteBigInt32(this Stream stream, int value)
        {
            stream.Write(new byte[] 
                {
                    (byte)(value >> 24),
                    (byte)(value >> 16),
                    (byte)(value >> 8),
                    (byte)(value)
                }, 0, 4);
        }
    }

    internal static class ArrayExtensions
    {
        public static T[] Append<T>(this T[] array, T value)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            T[] newArray = new T[array.Length + 1];
            newArray[array.Length] = value;
            array.CopyTo(newArray, 0);
            return newArray;
        }

        public static T[] RemoveAt<T>(this T[] array, int index)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index >= array.Length)
                throw new ArgumentOutOfRangeException("index");

            T[] newArray = new T[array.Length - 1];
            Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index + 1, newArray, index, newArray.Length - index);
            return newArray;
        }

        public static T[] Insert<T>(this T[] array, int index, T value)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            if (index < 0 || index > array.Length)
                throw new ArgumentOutOfRangeException("index");

            T[] newArray = new T[array.Length + 1];
            Array.Copy(array, 0, newArray, 0, index);
            Array.Copy(array, index, newArray, index + 1, array.Length - index);
            newArray[index] = value;
            return newArray;
        }
    }

    internal static class EnumEx
    {
        public static TEnum Parse<TEnum>(string value, bool ignoreCase)
        {
            return (TEnum)Enum.Parse(typeof(TEnum), value, ignoreCase);
        }
    }
}

namespace UAM.InformatiX.Collections
{
    internal static class EqualityComparer
    {
        private class DerivedComparer<T> : IEqualityComparer<T>
        {
            private IComparer<T> _comparer;

            public DerivedComparer(IComparer<T> comparer)
            {
                _comparer = comparer;
            }

            public bool Equals(T x, T y)
            {
                return _comparer.Compare(x, y) == 0;
            }

            public int GetHashCode(T obj)
            {
                return obj.GetHashCode();
            }
        }

        public static IEqualityComparer<T> FromComparer<T>(IComparer<T> comparer)
        {
            if (comparer == null)
                throw new ArgumentNullException("comparer");

            return new DerivedComparer<T>(comparer);
        }
    }
}

namespace UAM.InformatiX.Text.Json
{
    static class JsonParser
    {
        public static string EscapeString(string s)
        {
            int index = 0;
            int length = s.Length;

            StringBuilder encoded = new StringBuilder(length);
            for (int i = index; i < index + length; i++)
                switch (s[i])
                {
                    case '"': encoded.Append("\\\""); break;
                    case '\\': encoded.Append("\\\\"); break;
                    case '\b': encoded.Append("\\b"); break;
                    case '\f': encoded.Append("\\f"); break;
                    case '\n': encoded.Append("\\n"); break;
                    case '\r': encoded.Append("\\r"); break;
                    case '\t': encoded.Append("\\t"); break;
                    default:
                        if (char.IsControl(s[i])) // do not use IsControl(string, int) as in the case of control character we would have to split it again anyway
                            encoded.AppendFormat("\\u{0:X4}", (ushort)s[i]);
                        else
                            encoded.Append(s[i]);
                        break;
                }

            return encoded.ToString();
        }
    }
}


namespace UAM.Optics
{
    /// <summary>
    /// Represents a discretely sampled 2D signal.
    /// </summary>
    /// <typeparam name="TSample">Type of the samples.</typeparam>
    public interface ISampled2D<TSample>
    {
        /// <summary>
        /// Gets the sample at specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>the sample at specified coordinates.</returns>
        TSample this[int x, int y] { get; }

        /// <summary>
        /// Gets the sample count in x coordinate.
        /// </summary>
        int Width { get; }
        /// <summary>
        /// Gets the sample count in y coordinate.
        /// </summary>
        int Height { get; }
    }

    /// <summary>
    /// Represents a continuous 2D signal.
    /// </summary>
    /// <typeparam name="TSample"></typeparam>
    public interface IContinuous2D<TSample>
    {
        /// <summary>
        /// Gets signal value at specified coordinates.
        /// </summary>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>the signal value at specified coordinates.</returns>
        TSample this[double x, double y] { get; }
    }
}