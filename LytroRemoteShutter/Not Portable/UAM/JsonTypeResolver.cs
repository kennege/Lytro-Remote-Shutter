using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Reolves the JSON type names into CLR types and vice versa.
    /// </summary>
    public class JsonTypeResolver
    {
        /// <summary>
        /// Resolves the JSON type name into a CLR type.
        /// </summary>
        /// <param name="typeName">The JSON type name to resolve.</param>
        /// <returns>The CLR type corresponding to the JSON type name if found; otherwise null.</returns>
        public virtual Type ResolveType(string typeName)
        {
            if (typeName == null)
                return null;

            return Type.GetType(typeName, false);
        }

        /// <summary>
        /// Resolves the CLR type to a JSON type name.
        /// </summary>
        /// <param name="type">The CLR type to resolve.</param>
        /// <returns>The JSON type name corresponding to the CLR type if found; otherwise null.</returns>
        public virtual string ResolveTypeName(Type type)
        {
            if (type == null)
                return null;

            return type.Name;
        }

        /// <summary>
        /// Resolves the CLR type to a JSON type name.
        /// </summary>
        /// <typeparam name="T">The CLR type to resolve.</typeparam>
        /// <returns>The JSON type name corresponding to the CLR type if found; otherwise null.</returns>
        public string ResolveTypeName<T>()
        {
            return ResolveTypeName(typeof(T));
        }

        /// <summary>
        /// Returns an instance of the <see cref="JsonTypeResolver"/> which uses the mappings defined by the <see cref="JsonTypeMappingAttribute"/> on the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly where to look for the mappings.</param>
        /// <returns>An instance of the <see cref="JsonTypeResolver"/> which uses the mappings defined by the <see cref="JsonTypeMappingAttribute"/> on the specified assembly.</returns>
        public static JsonTypeResolver FromAssemblyMappings(Assembly assembly)
        {
            JsonTypeDictionary mappingsDict = new JsonTypeDictionary();
            mappingsDict.LoadAssemblyMappings(assembly);
            return mappingsDict;
        }
        /// <summary>
        /// Returns an instance of the <see cref="JsonTypeResolver"/> which uses the mappings defined by the <see cref="JsonTypeMappingAttribute"/> on the specified assembly.
        /// </summary>
        /// <typeparam name="T">Any type from the assembly where to look for the mappings.</typeparam>
        /// <returns>An instance of the <see cref="JsonTypeResolver"/> which uses the mappings defined by the <see cref="JsonTypeMappingAttribute"/> on the specified assembly.</returns>
        public static JsonTypeResolver FromAssemblyMappings<T>()
        {
            return FromAssemblyMappings(typeof(T).Assembly);
        }
    }
}
