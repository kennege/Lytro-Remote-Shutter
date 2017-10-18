using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Maps a JSON type to a CLR type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Assembly, AllowMultiple=true, Inherited=true)]
    public class JsonTypeMappingAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the JSON type name.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Gets or sets the CLR type to map the JSON type name to.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonTypeMappingAttribute"/> class.
        /// </summary>
        /// <param name="name">The JSON type name.</param>
        /// <param name="type">The CLR type to map the JSON type name to.</param>
        public JsonTypeMappingAttribute(string name, Type type)
        {
            Name = name;
            Type = type;
        }
    }
}
