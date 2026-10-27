using Rydia.Graphics.Api.ES30;
using Rydia.Runtime;
using Rydia.Drawing;
using Rydia.Graphics.Api.ES20;
using Rydia.Engine;

namespace Rydia.GameStudio.App
{
    internal class EditorApp : AppRunner // AppRunnerを継承してますが、LauncherAppを継承する可能性があります
    {

        public static IGLES20 GL
        {
            get
            {
                return RuntimeHost.GLES30;
            }
        }

        public SandboxApp SandboxApp
        {
            get;
            private set;
        }

        public EditorApp()
        {
            SandboxApp = new SandboxApp();
        }

        public override void Load()
        {
            SandboxApp.Init(Platform, Width, Height);
            SandboxApp.Load();
            GL.ClearColor(Rydia.Drawing.ColorRgba.CornflowerBlue);
        }

        public override void Update(float deltaTimeSec, float totalTimeSec)
        {
            SandboxApp.Update();
        }

        public override void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
            SandboxApp.Render();
        }

        public override void Unload()
        {
            SandboxApp.Unload();
        }

    }
}