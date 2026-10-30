using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rydia.Graphics.Api.ES30;
using Rydia.Runtime;
using Rydia.Runtime.Desktop;

namespace Rydia.Engine
{
    public class LauncherApp : AppRunner
    {

        public static IGLES30 GL
        {
            get
            {
                return GraphicsHost.GLES30;
            }
        }

        public override void Load()
        {
            
        }

        public override void Update(float deltaTimeSec, float totalTimeSec)
        {

        }

        public override void Render()
        {

        }

        public override void Unload()
        {

        }

    }
}
