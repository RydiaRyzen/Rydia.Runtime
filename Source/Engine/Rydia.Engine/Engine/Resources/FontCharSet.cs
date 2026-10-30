using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.Runtime.InteropServices;
using System.Text;
using Rydia.Serialization;

namespace Rydia.Engine.Resources
{

    /// <summary>
    /// 文字ごとの画像データを管理するリソースクラスです。
    /// 各文字に対応する<see cref="PixelData"/>を保持し、
    /// フォント描画に使用する文字画像データを提供します。
    /// </summary>
    public class FontCharSet : Resource
    {

        /// <summary>
        /// 文字とその文字画像データの組み合わせを表します。
        /// </summary>
        public class CharEntry
        {

            /// <summary>
            /// 対応する文字を取得または設定します。
            /// </summary>
            public char Character { get; set; }

            /// <summary>
            /// 文字に対応する画像データを取得または設定します。
            /// </summary>
            public PixelData PixelData { get; set; }
        }

        /// <summary>
        /// 文字と画像データの対応関係を保持します。
        /// </summary>
        private Dictionary<char, CharEntry> _CharSet = new();

        /// <summary>
        /// 登録されている文字と画像データの一覧を取得または設定します。
        /// シリアライズおよびデシリアライズに使用します。
        /// </summary>
        public int CharCount
        {
            get
            {
                return this._CharSet.Count;
            }
        }

        /// <summary>
        /// 指定された文字に画像データを登録します。
        /// 既に同じ文字が登録されている場合は、画像データを上書きします。
        /// </summary>
        /// <param name="v">登録する文字。</param>
        /// <param name="pixelData">文字に対応する画像データ。</param>
        public void Add(char v, PixelData pixelData)
        {
            this._CharSet[v] = new CharEntry
            {
                Character = v,
                PixelData = pixelData
            };
        }

        /// <summary>
        /// 指定された文字に対応する画像データを取得します。
        /// </summary>
        /// <param name="v">検索する文字。</param>
        /// <returns>
        /// 対応する画像データ。登録されていない場合は<see langword="null"/>。
        /// </returns>
        public PixelData GetPixelData(char v)
        {
            return this._CharSet.TryGetValue(v, out CharEntry entry)
                ? entry.PixelData : null;
        }

        /// <summary>
        /// 指定された文字に対応する画像データの取得を試みます。
        /// </summary>
        /// <param name="v">検索する文字。</param>
        /// <param name="pixelData">
        /// 取得した画像データ。取得できなかった場合は<see langword="null"/>。
        /// </param>
        /// <returns>
        /// 対応する文字が登録されている場合は<see langword="true"/>、
        /// それ以外の場合は<see langword="false"/>。
        /// </returns>
        public bool TryGetPixelData(char v, out PixelData pixelData)
        {
            var result = this._CharSet.TryGetValue(v, out CharEntry entry);
            pixelData = result ? entry.PixelData : null;
            return result;
        }

    }

}
