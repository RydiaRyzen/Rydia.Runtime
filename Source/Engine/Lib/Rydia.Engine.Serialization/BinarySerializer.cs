using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// バイナリ形式でオブジェクトをシリアライズおよび
    /// デシリアライズするクラスのベースクラスです。
    /// </summary>
    public class BinarySerializer : SerializerBase
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
