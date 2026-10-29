using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.Engine.Serialization
{

    /// <summary>
    /// シリアライズおよびデシリアライズするデータ型を表す列挙体です。
    /// </summary>
    public enum DataType
    {
        /// <summary>
        /// 未知のデータ型を表します。
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// 論理型（Boolean）を表します。
        /// </summary>
        Bool = 1,

        /// <summary>
        /// 符号なし8ビット整数型を表します。
        /// </summary>
        Byte = 2,

        /// <summary>
        /// 符号付き8ビット整数型を表します。
        /// </summary>
        SByte = 3,

        /// <summary>
        /// 符号付き16ビット整数型を表します。
        /// </summary>
        Short = 4,

        /// <summary>
        /// 符号なし16ビット整数型を表します。
        /// </summary>
        UShort = 5,

        /// <summary>
        /// 符号付き32ビット整数型を表します。
        /// </summary>
        Int = 6,

        /// <summary>
        /// 符号なし32ビット整数型を表します。
        /// </summary>
        UInt = 7,

        /// <summary>
        /// 符号付き64ビット整数型を表します。
        /// </summary>
        Long = 8,

        /// <summary>
        /// 符号なし64ビット整数型を表します。
        /// </summary>
        ULong = 9,

        /// <summary>
        /// 単精度浮動小数点数型を表します。
        /// </summary>
        Float = 10,

        /// <summary>
        /// 倍精度浮動小数点数型を表します。
        /// </summary>
        Double = 11,

        /// <summary>
        /// 10進浮動小数点数型を表します。
        /// </summary>
        Decimal = 12,

        /// <summary>
        /// 文字型を表します。
        /// </summary>
        Char = 13,

        /// <summary>
        /// 型情報（<see cref="System.Type"/>）を表します。
        /// </summary>
        Type = 14,

        /// <summary>
        /// デリゲート型を表します。
        /// </summary>
        Delegate = 20,

        /// <summary>
        /// 列挙型を表します。
        /// </summary>
        Enum = 21,

        /// <summary>
        /// 文字列型を表します。
        /// </summary>
        String = 22,

        /// <summary>
        /// 配列型を表します。
        /// </summary>
        Array = 23,

        /// <summary>
        /// 構造体型を表します。
        /// </summary>
        Struct = 25,

        /// <summary>
        /// オブジェクトへの参照を表します。
        /// </summary>
        ObjectRef = 26,

        /// <summary>
        /// メンバー情報（<see cref="System.Reflection.MemberInfo"/>）を表します。
        /// </summary>
        MemberInfo = 27,
    }
}
