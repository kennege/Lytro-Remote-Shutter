using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Threading;
using UAM.InformatiX.Collections;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Provides extension methods for reading and writing JSON strings.
    /// </summary>
    public static partial class JsonExtensions
    {
        private class TypeInKeyHashtable : Dictionary<string, object> { }

        #region JSON Output
        /// <summary>
        /// Returns a formatted JSON string representing the object.
        /// </summary>
        /// <param name="this">The object to represent.</param>
        /// <returns>A JSON string representing the object.</returns>
        public static string ToStringJson(this object @this)
        {
            return ToStringJson(@this, false, 0);
        }
        /// <summary>
        /// Returns a JSON string representing the object.
        /// </summary>
        /// <param name="this">The object to represent.</param>
        /// <param name="compact">true to exclude whitespace. Default is false.</param>
        /// <returns> a JSON string representing the object.</returns>
        public static string ToStringJson(this object @this, bool compact)
        {
            return ToStringJson(@this, compact, 0);
        }

        private static string ToStringJson(object o, bool compact, int level)
        {
            if (o == null)
                return "null";
            else if (false.Equals(o))
                return "false";
            else if (true.Equals(o))
                return "true";
            else if (o is string || o is char || o is Enum)
                return "\"" +  JsonEscape(o.ToString()) + "\"";
            else if (IsIDictionary(o))
                return IDictionaryToStringJson(ToIDictionary(o), compact, level);

            Type oType = o.GetType();
            if (oType.IsPrimitive || oType == typeof(decimal))
                return Convert.ToString(o, CultureInfo.InvariantCulture);

            JsonMemberCasing memberCasing = JsonMemberCasing.CamelCase;
            foreach (JsonMemberCasingAttribute attribute in oType.GetCustomAttributes<JsonMemberCasingAttribute>(true))
                memberCasing = attribute.Casing;

            StringBuilder sb = new StringBuilder();
            if (!compact && level > 0)
                sb.AppendLine();

            sb.Append(Indent(compact, level) + "{");
            ++level;

            bool firstItem = true;
            PropertyInfo[] properties = oType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < properties.Length; i++)
            {
                PropertyInfo item = properties[i];
                if (item.GetIndexParameters().Length > 0)
                    continue;

                object value = item.GetValue(o, null);

                if (value == null || object.ReferenceEquals(value, o))
                    continue;

                if ((value is bool && false.Equals(value)) ||
                    (value is decimal && decimal.Zero.Equals(value)) ||
                    object.Equals(value, value.GetType().GetDefault()))
                {
                    if (item.IsDefined<JsonIgnoreDefaultAttribute>(true))
                        continue;
                }

                if (!firstItem)
                    sb.Append(",");
                else
                    firstItem = false;

                if (!compact)
                    sb.AppendLine();

                string itemName = item.Name;
                switch (memberCasing)
                {
                    case JsonMemberCasing.LowerCase: itemName = itemName.ToLowerInvariant(); break;
                    case JsonMemberCasing.UpperCase: itemName = itemName.ToUpperInvariant(); break;
                    case JsonMemberCasing.CamelCase: itemName = itemName.ToCamelCaseInvariant(); break;
                }

                foreach (JsonMemberNameAttribute attribute in item.GetCustomAttributes<JsonMemberNameAttribute>(true))
                    if (attribute.IsOutputName)
                        itemName = attribute.JsonName ?? itemName;

                sb.Append(Indent(compact, level) + "\"" + itemName + "\":");

                if (!IsICollection(item.PropertyType) || IsIDictionary(item.PropertyType))
                {
                    if (!compact)
                        sb.Append(" ");

                    string valueJson = ToStringJson(value, compact, level);
                    sb.Append(valueJson);
                }
                else
                {
                    if (!compact)
                        sb.AppendLine();

                    sb.Append(Indent(compact, level) + "[");

                    if (!compact)
                        sb.AppendLine();

                    {
                        int j = 0;
                        ICollection valueCollection = ToICollection(value);
                        foreach (object valueItem in valueCollection)
                        {
                            sb.Append(Indent(compact, level + 1) + ToStringJson(valueItem, compact, level + 1));

                            if (++j < valueCollection.Count)
                                if (!compact)
                                    sb.AppendLine(",");
                                else
                                    sb.Append(",");
                        }

                        if (!compact && valueCollection.Count > 0)
                            sb.AppendLine();
                    }

                    sb.Append(Indent(compact, level) + "]");
                }
            }

            --level;

            if (!compact)
                sb.AppendLine();

            sb.Append(Indent(compact, level) + "}");

            return sb.ToString();
        }

        private static bool IsIDictionary(Type t)
        {
            if (typeof(IDictionary).IsAssignableFrom(t))
                return true;

            if (typeof(IEnumerable).IsAssignableFrom(t))
                foreach (Type type in t.GetInterfaces())
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        return true;

            return false;
        }
        private static bool IsIDictionary(object o)
        {
            if (o is IDictionary)
                return true;

            if (o is IEnumerable)
                foreach (Type type in o.GetType().GetInterfaces())
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                        return true;

            return false;
        }
        private static IDictionary ToIDictionary(object o)
        {
            if (o is IDictionary)
                return (IDictionary)o;

            foreach (Type type in o.GetType().GetInterfaces())
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IDictionary<,>))
                    return (IDictionary)typeof(NonGenericDictionaryWrapper<,>).MakeGenericType(type.GetGenericArguments()).GetConstructors()[0].Invoke(new object[] { o });

            throw new ArgumentException();
        }
        private static bool IsICollection(Type t)
        {
            if (typeof(ICollection).IsAssignableFrom(t))
                return true;

            if (typeof(IEnumerable).IsAssignableFrom(t))
                foreach (Type type in t.GetInterfaces())
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                        return true;

            return false;
        }
        private static bool IsICollection(object o)
        {
            if (o is ICollection)
                return true;

            if (o is IEnumerable)
                foreach (Type type in o.GetType().GetInterfaces())
                    if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                        return true;

            return false;
        }
        private static ICollection ToICollection(object o)
        {
            if (o is ICollection)
                return (ICollection)o;

            foreach (Type type in o.GetType().GetInterfaces())
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ICollection<>))
                    return (ICollection)typeof(NonGenericCollectionWrapper<>).MakeGenericType(type.GetGenericArguments()).GetConstructors()[0].Invoke(new object[] { o });

            throw new ArgumentException();
        }

        private static string IDictionaryToStringJson(IDictionary dict, bool compact, int level)
        {
            StringBuilder db = new StringBuilder();

            if (!compact)
                db.AppendLine();

            db.Append(Indent(compact, level) + "{");

            if (!compact)
                db.AppendLine();

            {
                int j = 0;
                foreach (DictionaryEntry entry in dict)
                {
                    db.Append(Indent(compact, level + 1) + "\"" + entry.Key.ToString().ToCamelCaseInvariant() + "\": ");

                    string valueJson = ToStringJson(entry.Value, compact, level + 1);
                    db.Append(valueJson);

                    if (++j < dict.Count)
                        if (!compact)
                            db.AppendLine(",");
                        else
                            db.Append(",");
                }

                if (!compact && dict.Count > 0)
                    db.AppendLine();
            }

            db.Append(Indent(compact, level) + "}");

            return db.ToString();
        }

        /// <summary>
        /// Returns an escaped string for use in JSON.
        /// </summary>
        /// <param name="s">The string to escape.</param>
        /// <returns>An escaped string for use in JSON.</returns>
        public static string JsonEscape(string s)
        {
            return JsonParser.EscapeString(s);
        }
        private static string Indent(bool compact, int level)
        {
            if (compact)
                return string.Empty;
            else
                return new string('\t', level);
        }
        #endregion

        #region JSON Input
        private enum JsonValueType
        {
            String,
            Number,
            Object,
            Array,
            True,
            False,
            Null
        }

        private class ResolverPending : IEquatable<ResolverPending>
        {
            public PropertyInfo TypeProperty;
            public PropertyInfo MemberProperty;
            public int I;

            public bool Equals(ResolverPending other)
            {
                return 
                    this.I == other.I &&
                    this.TypeProperty == other.TypeProperty;
            }
        }

        /// <summary>
        /// Parses a JSON string into an existing object using the default assembly type mappings.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse the JSON string into.</typeparam>
        /// <param name="this">The object to parse the JSON string into.</param>
        /// <param name="json">The JSON string to parse.</param>
        public static void LoadFromJson<T>(this T @this, string json)
        {
            LoadFromJson<T>(@this, json, null);
        }
        /// <summary>
        /// Parses a JSON string into an existing object.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse the JSON string into.</typeparam>
        /// <param name="this">The object to parse the JSON string into.</param>
        /// <param name="json">The JSON string to parse.</param>
        /// <param name="typeResolver">A class to help resolving dynamic or unknown types.</param>
        public static void LoadFromJson<T>(this T @this, string json, JsonTypeResolver typeResolver)
        {
            int start = 0;
            LoadFromJson(@this, json, ref start, typeResolver ?? JsonTypeResolver.FromAssemblyMappings<T>());
        }
        private static void LoadFromJson<T>(T target, string json, ref int i, JsonTypeResolver typeResolver)
        {
            if (target == null)
                throw new ArgumentNullException("target");

            if (json == null)
                throw new ArgumentNullException("json");

            Type targetType = typeof(T);
            if (target != null)
                targetType = target.GetType();

            IDictionary<string, PropertyInfo> memberResolver = GetAttributeMemberResolver(targetType);
            List<ResolverPending> resolverPendings = new List<ResolverPending>();

            SkipWhitespace(json, ref i);
            ReadObjectStart(json, ref i);

            do
            {
                SkipWhitespace(json, ref i);

                if (json[i] == '}')
                    break;

                string memberName = ReadMemberName(json, ref i);

                SkipWhitespace(json, ref i);
                ReadMemberValueSeparator(json, ref i);
                SkipWhitespace(json, ref i);

                if (target is IDictionary)
                {
                    IDictionary targetTable = (IDictionary)target;

                    Type expectedTableValueType = null;
                    if (targetTable is TypeInKeyHashtable)
                        expectedTableValueType = typeResolver.ResolveType(memberName);

                    targetTable.Add(memberName, ReadValue(json, ref i, null, expectedTableValueType, typeResolver));
                    goto nextMember;
                }

                PropertyInfo memberProperty = null;
                memberResolver.TryGetValue(memberName, out memberProperty);

            pendingMember:
                if (memberProperty == null || memberProperty.GetIndexParameters().Length > 0)
                {
                    global::System.Diagnostics.Debug.WriteLine("JSON Parser: Member '{0}' not found on the object of type '{1}' or requires indices. Skipping.", memberName, targetType);
                    ReadValue(json, ref i, null, null, typeResolver);
                    goto nextMember;
                }

                Type expectedValueType = memberProperty.PropertyType;
                if (typeResolver != null)
                    foreach (JsonMemberTypeInAttribute attribute in memberProperty.GetCustomAttributes<JsonMemberTypeInAttribute>(true))
                    {
                        PropertyInfo typeProperty = targetType.GetProperty(attribute.MemberName);
                        if (typeProperty != null)
                        {
                            if (typeProperty.PropertyType == typeof(Type))
                                expectedValueType = (Type)typeProperty.GetValue(target, null);
                            else if (typeProperty.PropertyType == typeof(string))
                                expectedValueType = typeResolver.ResolveType((string)typeProperty.GetValue(target, null)); // ?? memberProperty.PropertyType;

                            if (expectedValueType == null)
                            {
                                ResolverPending pending = new ResolverPending { TypeProperty = typeProperty, MemberProperty = memberProperty, I = i };

                                if (!resolverPendings.Any(p => p == pending))
                                    resolverPendings.Add(pending);

                                ReadValue(json, ref i, null, null, null);
                                goto nextMember;
                            }
                        }
                        else
                            global::System.Diagnostics.Debug.WriteLine("Type property '{0}' not found on object of type '{1}'. Ignoring attribute.", attribute.MemberName, targetType.Name);
                    }

                if (expectedValueType.Implements<IDictionary>() && memberProperty.GetCustomAttributes<JsonMemberTypeInKeyAttribute>(true).Any())
                    expectedValueType = typeof(TypeInKeyHashtable);

                object currentValue = memberProperty.CanRead ? memberProperty.GetValue(target, null) : null;
                object value = ReadValue(json, ref i, currentValue, expectedValueType, typeResolver);

                if (!memberProperty.CanWrite)
                {
                    if (value != currentValue)
                        global::System.Diagnostics.Debug.WriteLine("Member '{0}' is read-only. Skipping.", memberName);

                    goto nextMember;
                }

                if (value != null)
                {
                    Type valueType = value.GetType();

                    if (!memberProperty.PropertyType.IsAssignableFrom(valueType))
                        try { value = System.Convert.ChangeType(value, memberProperty.PropertyType, Thread.CurrentThread.CurrentCulture); }
                        catch (Exception e)
                        {
                            global::System.Diagnostics.Debug.WriteLine("Invalid value for member '{0}' of type '{1}'. Skipping. ({2})", memberName, value, e.Message);
                            goto nextMember;
                        }
                }

                try
                {
                    object oldValue = memberProperty.GetValue(target, null);
                    memberProperty.SetValue(target, value, null);

                    ResolverPending pending = resolverPendings.FirstOrDefault(p => p.TypeProperty == memberProperty);
                    if (pending != null)
                    {
                        resolverPendings.Remove(pending);

                        if (!object.Equals(oldValue, value))
                        {
                            i = pending.I;
                            memberProperty = pending.MemberProperty;

                            global::System.Diagnostics.Debug.WriteLine("Second pass for resolving value type for '{0}'.", memberProperty);
                            goto pendingMember;
                        }
                        else
                        {
                            global::System.Diagnostics.Debug.WriteLine("Value type for '{0}' cannot be resolved. Skipping.", memberProperty);
                        }
                    }
                }
                catch (Exception e)
                {
                    global::System.Diagnostics.Debug.WriteLine("Cannot set value '{0}' to member '{1}'. Skipping. ({2}).", value, memberName, e.Message);
                    goto nextMember;
                }

            nextMember:
                SkipWhitespace(json, ref i);

                if (json[i] == '}')
                    break;
                else if (json[i] != ',')
                    throw new FormatException("Unexpected character '" + json[i] + "' at position " + i + ".");

                ++i;
            } while (true);

            ++i; // move after curly bracket

            foreach (ResolverPending pending in resolverPendings)
                global::System.Diagnostics.Debug.WriteLine("Cannot set value '{0}' because the expected type in '{1}' was not supplied. Skipping.", pending.MemberProperty, pending.TypeProperty);

        }

        /// <summary>
        /// Parses a JSON snippet of a value using the default assembly type mappings.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse.</typeparam>
        /// <param name="json">The string containing the JSON snippet.</param>
        /// <returns>The parsed value.</returns>
        public static T LoadObject<T>(string json) where T : new()
        {
            return LoadObject<T>(json, null);
        }
        /// <summary>
        /// Parses a JSON snippet of a value.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse.</typeparam>
        /// <param name="json">The string containing the JSON snippet.</param>
        /// <param name="typeResolver">A class to help resolving dynamic or unknown types.</param>
        /// <returns>The parsed value.</returns>
        public static T LoadObject<T>(string json, JsonTypeResolver typeResolver) where T : new()
        {
            int i = 0;
            return (T)ReadValue(json, ref i, null, typeof(T), typeResolver ?? JsonTypeResolver.FromAssemblyMappings<T>());
        }

        /// <summary>
        /// Parses a JSON snippet of an array value using the default assembly type mappings.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse the array items into.</typeparam>
        /// <param name="json">The string containing the JSON snippet, begining and ending with square brackets.</param>
        /// <returns>The parsed array.</returns>
        public static T[] LoadArray<T>(string json)
        {
            return LoadArray<T>(json, null);
        }
        /// <summary>
        /// Parses a JSON snippet of an array value.
        /// </summary>
        /// <typeparam name="T">The type of the object to parse the array items into.</typeparam>
        /// <param name="json">The string containing the JSON snippet, begining and ending with square brackets.</param>
        /// <param name="typeResolver">A class to help resolving dynamic or unknown types.</param>
        /// <returns>The parsed array.</returns>
        public static T[] LoadArray<T>(string json, JsonTypeResolver typeResolver)
        {
            int i = 0;

            IList list = ReadArray(json, ref i, typeof(T[]), typeResolver ?? JsonTypeResolver.FromAssemblyMappings<T>(), true);
            T[] array = new T[list.Count];

            list.CopyTo(array, 0);
            return array;
        }

        private static Dictionary<string, PropertyInfo> GetAttributeMemberResolver(Type targetType)
        {
            Dictionary<string, PropertyInfo> memberResolver = new Dictionary<string, PropertyInfo>(StringComparer.OrdinalIgnoreCase);
            
            PropertyInfo[] properties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo property in properties)
            {
                bool nameCustomized = false;

                foreach (JsonMemberNameAttribute attribute in property.GetCustomAttributes<JsonMemberNameAttribute>(true))
                {
                    memberResolver[attribute.JsonName ?? property.Name] = property;
                    nameCustomized = true;
                }

                if (!nameCustomized)
                    memberResolver[property.Name] = property;
            }

            return memberResolver;
        }

        private static object ReadValue(string s, ref int i, object value, Type expectedValueType, JsonTypeResolver typeResolver)
        {
            JsonValueType jsonType = ReadValueType(s, ref i);

            switch (jsonType)
            {
                case JsonValueType.String:
                    value = ReadString(s, ref i);
                    break;

                case JsonValueType.Number:
                    value = ReadNumber(s, ref i);
                    break;

                case JsonValueType.Object:
                    if (value == null)
                    {
                        if (expectedValueType != null)
                        {
                            ConstructorInfo memberCtor = expectedValueType.GetConstructor(new Type[0]);

                            if (memberCtor == null)
                                throw new FormatException("A type with parameter-less constructor is required at position " + i + ". If not available, supply an instance as the member's value.");

                            value = memberCtor.Invoke(null);
                        }
                        else
                            value = new Dictionary<string, object>();
                    }

                    LoadFromJson(value, s, ref i, typeResolver);
                    break;

                case JsonValueType.Array:
                    List<object> arrayValues = ReadArray(s, ref i, expectedValueType, typeResolver);

                    if (expectedValueType != null && expectedValueType.IsArray)
                    {
                        Array array = Array.CreateInstance(expectedValueType.GetElementType(), arrayValues.Count);

                        object[] genericArray = arrayValues.ToArray();
                        genericArray.CopyTo(array, 0);
                        value = array;
                    }
                    else if (value is IList)
                    {
                        IList valueList = (IList)value;
                        try
                        {
                            valueList.Clear();
                            foreach (object arrayValue in arrayValues)
                                valueList.Add(arrayValue);
                        }
                        catch (Exception e)
                        {
                            global::System.Diagnostics.Debug.WriteLine("Array values could not be loaded at position {0}. Skipping. ({1})", i, e.Message);
                        }
                    }
                    else if (expectedValueType == null || expectedValueType.IsAssignableFrom(typeof(IList)))
                    {
                        value = arrayValues;
                    }
                    else if (expectedValueType.Implements<IList>())
                    {
                        ConstructorInfo memberCtor = expectedValueType.GetConstructor(new Type[0]);

                        if (memberCtor == null)
                            throw new FormatException("A type with parameter-less constructor is required at position " + i + ". If not available, supply an instance as the member's value.");

                        IList valueList = (IList)memberCtor.Invoke(null);
                        try
                        {
                            valueList.Clear();
                            foreach (object arrayValue in arrayValues)
                                valueList.Add(arrayValue);
                        }
                        catch (Exception e)
                        {
                            global::System.Diagnostics.Debug.WriteLine("Array values could not be loaded at position {0}. Skipping. ({1})", i, e.Message);
                        }

                        value = valueList;
                    }
                    else
                    {
                        throw new NotSupportedException("'" + expectedValueType + "' is not a supported type for loading array values.");
                    }
                    break;

                case JsonValueType.True:
                    value = true;
                    break;

                case JsonValueType.False:
                    value = false;
                    break;

                case JsonValueType.Null:
                    value = null;
                    break;

                default:
                    throw new NotSupportedException("Unsupported JSON type.");
            }
            return value;
        }

        private static List<object> ReadArray(string s, ref int i, Type expectedValueType, JsonTypeResolver typeResolver, bool standalone = false)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);
            global::System.Diagnostics.Debug.Assert(s[i] == '[');

            List<object> values = new List<object>();
            Type expectedArrayValueType = null;

            if (expectedValueType != null)
            {
                Type[] listTypes;

                if (expectedValueType.HasElementType)
                    expectedArrayValueType = expectedValueType.GetElementType();

                else if (expectedValueType.Implements(typeof(IList<>), out listTypes))
                    expectedArrayValueType = listTypes[0].GenericTypeArguments[0];
            }

            ++i;
            EnsureNotEndOfString(s, i);
            SkipWhitespace(s, ref i);

            if (s[i] != ']')
                while (true)
                {
                    object arrayValue = ReadValue(s, ref i, null, expectedArrayValueType, typeResolver);
                    values.Add(arrayValue);

                    SkipWhitespace(s, ref i);
                    if (s[i] == ']')
                        break;

                    ReadNextMemberSeparator(s, ref i);
                    SkipWhitespace(s, ref i);
                }

            ++i;

            //if (!standalone)
            //    EnsureNotEndOfString(s, i);

            return values;
        }

        private static JsonValueType ReadValueType(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            if (char.IsDigit(s, i) || s[i] == '-') return JsonValueType.Number;
            if (s[i] == '"') return JsonValueType.String;
            if (s[i] == '{') return JsonValueType.Object;
            if (s[i] == '[') return JsonValueType.Array;

            if (s.Length > i + 5)
            {
                if (s.Substring(i, 4) == "null") { i += 4; EnsureNotEndOfString(s, i); return JsonValueType.Null; }
                if (s.Substring(i, 4) == "true") { i += 4; EnsureNotEndOfString(s, i); return JsonValueType.True; }
                if (s.Substring(i, 5) == "false") { i += 5; EnsureNotEndOfString(s, i); return JsonValueType.False; }
            }

            throw new FormatException("Expected value at position " + i + ".");
        }
        private static string ReadMemberName(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            if (s[i] != '"')
                throw new FormatException("Expected '\"' at position " + i + ".");

            return ReadString(s, ref i);

            /*
            if (!char.IsLetter(s, i))
                throw new FormatException("Expected letter at position " + i + ".");

            int start = i;

            while (i < s.Length)
                if (!char.IsLetterOrDigit(s, i++))
                    break;

            global::System.Diagnostics.Debug.Assert(start < i);
            EnsureNotEndOfString(s, i);

            return s.Substring(start, i - start);
            */
        }

        private static void ReadObjectStart(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            if (s[i] != '{')
                throw new FormatException("Expected '{' at position " + i + ".");

            ++i;
            EnsureNotEndOfString(s, i);
        }
        private static void ReadMemberValueSeparator(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            if (s[i] != ':')
                throw new FormatException("Expected ':' at position " + i + ".");

            ++i;
            EnsureNotEndOfString(s, i);
        }
        private static void ReadNextMemberSeparator(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            if (s[i] != ',')
                throw new FormatException("Expected ',' at position " + i + ".");

            ++i;
            EnsureNotEndOfString(s, i);
        }

        private static decimal ReadNumber(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);
            global::System.Diagnostics.Debug.Assert(char.IsDigit(s, i) || s[i] == '-');

            int start = i;

            while (++i < s.Length)
                if (!char.IsDigit(s, i) && s[i] != '.' && s[i] != '-' && s[i] != '+' && s[i] != 'e' && s[i] != 'E')
                    break;

            decimal value;

            if (!decimal.TryParse(s.Substring(start, i - start), NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent, CultureInfo.InvariantCulture, out value))
                throw new FormatException("Invalid number at position " + i + ".");

            //++i; // above
            //EnsureNotEndOfString(s, i);

            return value;
        }
        private static string ReadString(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);
            global::System.Diagnostics.Debug.Assert(s[i] == '"');

            StringBuilder value = new StringBuilder();

            ++i;
            EnsureNotEndOfString(s, i);

            while (true)
            {
                if (s[i] == '"')
                {
                    ++i;
                    //EnsureNotEndOfString(s, i);

                    return value.ToString();
                }

                if (s[i] != '\\')
                    value.Append(s[i++]);

                else
                {
                    ++i;
                    EnsureNotEndOfString(s, i);

                    switch (s[i])
                    {
                        case '"': value.Append('"'); ++i; break;
                        case '\\': value.Append('\\'); ++i; break;
                        case '/': value.Append('/'); ++i; break;
                        case 'b': value.Append('\b'); ++i; break;
                        case 'f': value.Append('\f'); ++i; break;
                        case 'n': value.Append('\n'); ++i; break;
                        case 'r': value.Append('\r'); ++i; break;
                        case 't': value.Append('\t'); ++i; break;
                        case 'u':
                            byte[] unicodeBytes = ReadSubsequentEscapedChars(s, ref i);
                            value.Append(Encoding.Unicode.GetString(unicodeBytes, 0, unicodeBytes.Length));
                            break;

                        default:
                            throw new FormatException("Unrecognized escape character '" + s[i] + "' at position " + i + ".");
                    }
                }
            }
        }

        private static byte[] ReadSubsequentEscapedChars(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);
            global::System.Diagnostics.Debug.Assert(s[i] == 'u');

            List<byte> bytes = new List<byte>(4);

            while (true)
            {
                EnsureNotEndOfString(s, i + 5);

                string hexValue = s.Substring(i + 1, 4);

                int value;

                if (!int.TryParse(hexValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out value))
                    throw new FormatException("Invalid hexadecimal sequence at position " + i + ".");

                bytes.Add((byte)value);
                bytes.Add((byte)(value >> 8));

                i += 5;

                if (i >= s.Length || s[i] != '\\')
                    break;

                if (i + 1 >= s.Length || s[i + 1] != 'u')
                    break;

                ++i;
            }

            return bytes.ToArray();
        }

        private static void SkipWhitespace(string s, ref int i)
        {
            global::System.Diagnostics.Debug.Assert(s != null);
            global::System.Diagnostics.Debug.Assert(i >= 0 && i < s.Length);

            while (i < s.Length)
                if (!char.IsWhiteSpace(s, i))
                    break;
                else
                    ++i;

            EnsureNotEndOfString(s, i);
        }
        private static void EnsureNotEndOfString(string s, int i)
        {
            if (i >= s.Length)
                throw new ArgumentException("Unexpected end of string.");
        }

        #endregion
    }
}
