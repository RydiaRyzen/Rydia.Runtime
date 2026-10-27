using System;
using System.Collections.Generic;
using System.Text;
using Rydia.Graphics.Api.ES30;
using Rydia.Runtime;

namespace Rydia.GameStudio.App
{

    internal class SandboxApp : AppRunner
    {

        public static IGLES30 GL
        {
            get
            {
                return RuntimeHost.GLES30;
            }
        }

        public override void Load()
        {

        }

        public override void Render()
        {

        }

        public override void Unload()
        {

        }

        public override void Update(float deltaTimeSec, float totalTimeSec)
        {

        }
    }

}
