using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    public sealed class BinaryResourceSerializer<TResource>
    : ResourceSerializer<TResource>
    where TResource : Resource
    {

        protected override IDataWriter Writer
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        protected override IDataReader Reader
        {
            get
            {
                throw new NotImplementedException();
            }
        }

    }

}
