using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using System.Windows.Forms;
using WeifenLuo.WinFormsUI.Docking;
using OpenTK;
using Rydia.Graphics.Api.ES30;
using Rydia.Graphics.Api.ES20;
using Rydia.Runtime.Windows;
using Rydia.GameStudio.App;
using Rydia.Audio.Api.OpenAL;
using Rydia.Runtime;
using System.ComponentModel;
using Rydia.Drawing;
using Microsoft.VisualBasic.Devices;
using Rydia.Input;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Rydia.GameStudio
{

    /// <summary>
    /// シーンを表示するエディタウィンドウを表します。
    /// </summary>
    public sealed class SceneView : DockContent
    {

        private readonly GLControl glControl;
        private readonly Panel scenePanel;

        /// <summary>
        /// OpenGL の描画領域を取得します。
        /// </summary>
        public GLControl GLControl
        {
            get { return this.glControl; }
        }

        public static IGLES30 GL
        {
            get
            {
                return GraphicsHost.GLES30;
            }
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public static IPlatform Platform
        {
            get;
            private set;
        }

        /// <summary>
        /// 新しい SceneView を初期化します。
        /// </summary>
        public SceneView()
        {
            Text = "Scene";
            BackColor = Color.FromArgb(30, 32, 39);
            ForeColor = Color.White;
            ShowHint = DockState.Document;

            this.scenePanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 26, 32)
            };

            this.glControl = new GLControl
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(24, 26, 32),
                VSync = true
            };

            this.glControl.Load += GLControl_Load;
            this.glControl.Resize += GLControl_Resize;

            this.scenePanel.Controls.Add(this.glControl);
            Controls.Add(this.scenePanel);

            var platform = new WindowsPlatform();
            platform.App = new EditorApp();
            AudioHost.CreateDeviceContext(platform.AudioOptions);
            SceneView.Platform = platform;

            platform.Keyboard.Source = new ControlKeyboardInputSource(GLControl);
            platform.Mouse.Source = new ControlMouseInputSource(GLControl);

            FormClosed += HandleFormClosed;
            platform.App.Init(new CrossPlatform(platform), GLControl.Width, GLControl.Height);
        }

        public void MainLoop()
        {
            if (!this.glControl.IsHandleCreated ||
            this.glControl.ClientSize.Width <= 0 ||
            this.glControl.ClientSize.Height <= 0)
            {
                return;
            }

            this.glControl.MakeCurrent();
            Platform.App.Update();
            Platform.App.Render();

            // TODO: シーンの描画処理をここに実装します。

            this.glControl.SwapBuffers();
        }

        private void HandleFormClosed(object? sender, FormClosedEventArgs e)
        {
            Platform.App.Unload();
        }

        private void GLControl_Load(object sender, EventArgs e)
        {
            this.glControl.MakeCurrent();
            Platform.App.Load();
        }

        private void GLControl_Resize(object sender, EventArgs e)
        {
            if (!this.glControl.IsHandleCreated)
                return;

            this.glControl.MakeCurrent();
            Platform.App.Resize(GLControl.Width, GLControl.Height);
            this.glControl.Invalidate();
        }

    }

}
