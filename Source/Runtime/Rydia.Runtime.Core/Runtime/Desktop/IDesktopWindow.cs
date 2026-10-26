using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Input;

namespace Rydia.Runtime.Desktop
{

    /// <summary>
    /// デスクトップ環境で使用するウィンドウを定義します。
    /// </summary>
    public interface IDesktopWindow : IDisposable
    {

        /// <summary>
        /// ウィンドウのメインループを実行します。
        /// </summary>
        /// <param name="v">フレーム更新間隔を秒単位で指定します。</param>
        void Run(double v);

    }

}
