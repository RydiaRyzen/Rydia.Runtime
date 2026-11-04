using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Engine.Serialization;

namespace Rydia.Engine
{
    public class Texture : Resource
    {


        public ContentRef<Pixmap> BasePixmap
        {
            get;
            set;
        }

    }
}
