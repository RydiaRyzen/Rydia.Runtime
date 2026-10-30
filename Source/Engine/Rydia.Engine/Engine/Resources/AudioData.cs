using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Serialization;

namespace Rydia.Engine
{

    /// <summary>
    /// PCM形式の音声データを保持するリソースクラスです。
    /// チャンネル数、サンプリングレート、およびPCMデータを管理します。
    /// </summary>
    public sealed class AudioData : Resource
    {

        /// <summary>
        /// 音声のチャンネル数を取得または設定します。
        /// </summary>
        public int Channels
        {
            get;
            set;
        }

        /// <summary>
        /// 音声のサンプリングレートをHz単位で取得または設定します。
        /// </summary>
        public int SampleRate
        {
            get;
            set;
        }

        /// <summary>
        /// 16ビット符号付き整数形式のPCM音声データを取得または設定します。
        /// </summary>
        public short[] PCMData
        {
            get;
            set;
        }

    }

}
