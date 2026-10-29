using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization.Tests
{
    public class TestObject2 : Resource
    {

        public int Level { get; set; }

        public float Health { get; set; }

        public short[] Status
        {
            get;
            set;
        }

    }
}
