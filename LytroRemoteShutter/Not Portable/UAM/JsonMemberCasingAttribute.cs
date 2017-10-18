using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Specifies the default casing when serializing this JSON object.
    /// </summary>
    /// <remarks>
    /// Use <see cref="JsonMemberNameAttribute"/> to override serialized name on individual members.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct, AllowMultiple = false, Inherited = true)]
    public class JsonMemberCasingAttribute : Attribute
    {
        readonly JsonMemberCasing _casing;
        /// <summary>
        /// Gets the casing of the member when serializing into JSON.
        /// </summary>
        public JsonMemberCasing Casing { get { return _casing; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMemberCasingAttribute"/> class.
        /// </summary>
        /// <param name="casing">The casing of members when serializing into JSON.</param>
        public JsonMemberCasingAttribute(JsonMemberCasing casing)
        {            
            _casing = casing;
        }
    }

    /// <summary>
    /// Specifies the member casing. 
    /// </summary>
    public enum JsonMemberCasing
    {
        /// <summary>
        /// Preserve casing as declared.
        /// </summary>
        Preserve,
        /// <summary>
        /// Output property names in lower case.
        /// </summary>
        LowerCase,
        /// <summary>
        /// Output property names in upper case.
        /// </summary>
        UpperCase,
        /// <summary>
        /// Output property names in camel case.
        /// </summary>
        CamelCase
    }
}
