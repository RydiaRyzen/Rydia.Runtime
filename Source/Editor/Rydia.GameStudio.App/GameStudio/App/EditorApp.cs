using Rydia.Graphics.Api.ES30;
using Rydia.Runtime;
using Rydia.Drawing;
using Rydia.Graphics.Api.ES20;
using Rydia.Engine;

namespace Rydia.GameStudio.App
{
    internal class EditorApp : AppRunner // AppRunnerを継承してますが、LauncherAppを継承する可能性があります
    {

        // IGLESを宣言してますが、IGraphicsDebiceを使用して、描画する必要があります

        public static IGLES20 GL
        {
            get
            {
                return GraphicsHost.GLES30;
            }
        }

        public EditorApp()
        {

        }

        public override void Load()
        {
            GL.ClearColor(Rydia.Drawing.ColorRgba.CornflowerBlue);
        }

        public override void Update(float deltaTimeSec, float totalTimeSec)
        {

        }

        public override void Render()
        {
            GL.Clear(ClearBufferMask.ColorBufferBit);
        }

        public override void Unload()
        {

        }

    }
}