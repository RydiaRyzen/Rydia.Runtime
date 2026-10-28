using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{

    /// <summary>
    /// バイナリファイルの定義を表します。
    /// </summary>
    public class BinaryFileDefinition
    {

        /// <summary>
        /// 指定されたファイル名とバイナリデータを使用して、
        /// <see cref="BinaryFileDefinition"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <param name="bytes">ファイルのバイナリデータ。</param>
        public BinaryFileDefinition(string fileName, byte[] bytes)
        {
            FileName = fileName;
            Bytes = bytes;
        }

        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// ファイルのバイナリデータを取得します。
        /// </summary>
        public byte[] Bytes { get; }

    }
}
