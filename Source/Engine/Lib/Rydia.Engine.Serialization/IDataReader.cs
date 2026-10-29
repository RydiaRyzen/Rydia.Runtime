using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// シリアライズされたデータを読み取るためのインターフェースを定義します。
    /// </summary>
    public interface IDataReader
    {
        /// <summary>
        /// 読み取り可能なデータのキー一覧を取得します。
        /// </summary>
        IEnumerable<string> Keys { get; }

        /// <summary>
        /// 指定された名前のデータを読み取ります。
        /// </summary>
        /// <param name="name">読み取るデータの名前。</param>
        /// <returns>読み取ったデータ。</returns>
        object ReadValue(string name);

        /// <summary>
        /// 指定された名前のデータを指定した型で読み取ります。
        /// </summary>
        /// <typeparam name="T">読み取るデータの型。</typeparam>
        /// <param name="name">読み取るデータの名前。</param>
        /// <returns>読み取ったデータ。</returns>
        T ReadValue<T>(string name);

        /// <summary>
        /// 指定された名前のデータを読み取り、出力引数に格納します。
        /// </summary>
        /// <typeparam name="T">読み取るデータの型。</typeparam>
        /// <param name="name">読み取るデータの名前。</param>
        /// <param name="value">読み取ったデータ。</param>
        void ReadValue<T>(string name, out T value);
    }

}
