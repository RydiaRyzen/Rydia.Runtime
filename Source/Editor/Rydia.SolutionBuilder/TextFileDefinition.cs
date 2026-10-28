using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{

    /// <summary>
    /// テキストファイルの定義を表します。
    /// </summary>
    public class TextFileDefinition
    {

        /// <summary>
        /// 指定されたファイル名とテキストを使用して、
        /// <see cref="TextFileDefinition"/> クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="fileName">ファイル名。</param>
        /// <param name="text">ファイルに書き込むテキスト。</param>
        public TextFileDefinition(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }

        /// <summary>
        /// ファイル名を取得します。
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// ファイルに書き込むテキストを取得します。
        /// </summary>
        public string Text { get; }

    }
}
