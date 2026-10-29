using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
    public class DontSerializeAttribute : Attribute { }

}
