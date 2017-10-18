using System;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Prevents including a member in the JSON string if it equals to the default value of its type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class JsonIgnoreDefaultAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonIgnoreDefaultAttribute"/> class.
        /// </summary>
        public JsonIgnoreDefaultAttribute()
        {

        }
    }
}
