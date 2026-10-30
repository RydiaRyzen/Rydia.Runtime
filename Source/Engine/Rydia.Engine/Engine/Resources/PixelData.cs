using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Drawing;
using Rydia.Serialization;

namespace Rydia.Engine
{

    /// <summary>
    /// 単一の画像データを表すリソースクラスです。
    /// <see cref="PixelData"/>は画像の幅、高さ、およびRGBA形式のピクセルデータを保持します。
    /// <see cref="Pixmap"/>は複数の<see cref="PixelData"/>を格納し、
    /// アトラスを設定することで、チップ画像として利用するための機能を提供します。
    /// </summary>
    public class PixelData : Resource
    {

        /// <summary>
        /// 画像の幅をピクセル単位で取得または設定します。
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 画像の高さをピクセル単位で取得または設定します。
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// 画像のピクセルデータをRGBA形式のバイト配列として取得または設定します。
        /// 各ピクセルはR、G、B、Aの順に4バイトで格納されます。
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 指定した色と一致するピクセルを完全な透明色に変更します。
        /// </summary>
        /// <param name="color">透明化する対象の色。</param>
        /// <remarks>
        /// ピクセルデータが設定されていない場合、または
        /// データの長さが画像サイズに対応していない場合は処理を行いません。
        /// </remarks>
        public void Transparent(ColorRgba color)
        {
            if (Data == null || Data.Length != Width * Height * 4)
                return;
            for (int i = 0; i < Data.Length; i += 4)
            {
                if (Data[i] == color.R && Data[i + 1] == color.G && Data[i + 2] == color.B && Data[i + 3] == color.A)
                {
                    Data[i] = 0;     // R
                    Data[i + 1] = 0; // G
                    Data[i + 2] = 0; // B
                    Data[i + 3] = 0; // A (fully transparent)
                }
            }
        }

    }

}
