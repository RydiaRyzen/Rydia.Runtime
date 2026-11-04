using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.Win32
{
    /// <summary>
    /// キャプチャオプションを表す列挙体です
    /// </summary>
    public enum PrintWindowCommand
    {
        /// <summary>
        /// キャプション領域等を含む矩形をキャプチャします
        /// </summary>
        None = 0,
        /// <summary>
        /// クライアント領域のみをキャプチャします
        /// </summary>
        ClientOnly = 1,
    }
}
