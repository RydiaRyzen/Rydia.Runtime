using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.Win32
{
    [StructLayout(LayoutKind.Sequential, Pack = 4)]
    public struct RECT
    {
        public int Left;

        public int Top;

        public int Right;

        public int Bottom;
    }
}
