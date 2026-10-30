using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// 独自のデータシリアライズ処理を提供するインターフェースです。
    /// </summary>
    public interface ICustomSerializable
    {

        /// <summary>
        /// データを書き込みます。
        /// </summary>
        /// <param name="writer">データを書き込むための Writer。</param>
        void WriteData(IDataWriter writer);

        /// <summary>
        /// データを読み込みます。
        /// </summary>
        /// <param name="reader">データを読み取るための Reader。</param>
        void ReadData(IDataReader reader);

    }

}
