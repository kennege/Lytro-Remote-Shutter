using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Specifies that the type information needed for parsing this JSON object is stored in another member.
    /// </summary>
    /// <remarks>
    /// If the member is parsed prior to the required type information, the value is discarded and reparsed once the type information is available.
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class JsonMemberTypeInAttribute : Attribute
    {
        readonly string _memberName;
        /// <summary>
        /// Gets the name of the member storing the type information.
        /// </summary>
        public string MemberName { get { return _memberName; } }

        /// <summary>
        /// Initializes a new instance of the <see cref="JsonMemberTypeInAttribute"/> class.
        /// </summary>
        /// <param name="memberName">The name of the member where the type information is stored.</param>
        public JsonMemberTypeInAttribute(string memberName)
        {
            _memberName = memberName;
        }
    }
}
