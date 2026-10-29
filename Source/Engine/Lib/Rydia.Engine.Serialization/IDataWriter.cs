using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// シリアライズするデータを書き込むためのインターフェースを定義します。
    /// </summary>
    public interface IDataWriter
    {
        /// <summary>
        /// 指定された名前でデータを書き込みます。
        /// </summary>
        /// <param name="name">書き込むデータの名前。</param>
        /// <param name="value">書き込むデータ。</param>
        void WriteValue(string name, object value);
    }

}
