using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization.Tests
{
    internal class TestObject1 : Resource
    {

        public int Level { get; set; }

        public float Health { get; set; }

        public int[] Status
        {
            get;
            set;
        }

        public ContentRef<TestObject2> Obj
        {
            get;
            set;
        }

    }
}
