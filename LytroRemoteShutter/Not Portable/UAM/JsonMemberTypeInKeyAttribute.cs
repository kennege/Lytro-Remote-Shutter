using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UAM.InformatiX.Text.Json
{
    /// <summary>
    /// Specifies that the type information needed for parsing this JSON object's members is stored in the member names.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple=false, Inherited=true)]
    public class JsonMemberTypeInKeyAttribute : Attribute
    {

    }
}
