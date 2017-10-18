using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Allows to specify different names for the member in a JSON string.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true, Inherited = true)]
    public class JsonMemberNameAttribute : Attribute
    {
        /// <summary>
        /// Gets or sets the name of the member in a JSON string.
        /// </summary>
        public string JsonName { get; set; }
        /// <summary>
        /// Gets or sets whether the name should be used in a generated JSON string as well.
        /// </summary>
        public bool IsOutputName { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMemberNameAttribute"/> class.
        /// </summary>
        public JsonMemberNameAttribute()
        {

        }
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMemberNameAttribute"/> class with specified name.
        /// </summary>
        /// <param name="jsonName">The name to use in a JSON string.</param>
        public JsonMemberNameAttribute(string jsonName)
        {
            JsonName = jsonName;
        }
    }
}
