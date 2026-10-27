

using Rydia.Diagnostics;
using Rydia.Engine;
using Rydia.Runtime.Desktop;
using Rydia.Runtime.Windows;

namespace Rydia.GameStudio.App
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            GlobalLogger.AddListener(new EditorLogListener());
            var fileLog = new FileLogListener("Logs\\Rydia.GameStudio.App.log");
            GlobalLogger.AddListener(fileLog);
            using (var splash = new SplashForm())
            {
                if (splash.ShowDialog() != DialogResult.OK)
                    return;
                
                using (var hub = new HubForm())
                {
                    if (hub.ShowDialog() != DialogResult.OK)
                        return;
                }
            }

            // ImGuiベースのアプリケーションを起動するか、WinFormsベースのアプリケーションを起動するかを切り替えるフラグ
            var onImGui = false;
            // サンプル用アプリケーションを起動するかどうかを切り替えるフラグ
            var isSample = true;

            if(!onImGui)
            {
                if (isSample)
                {
                    //**************************************************************//
                    //* ここではサンプル用のメインフォームを起動します             *//
                    //**************************************************************//
                    var form = new SampleMainForm();
                    //**************************************************************//
                    //* メインループを実行するために、MessagePump.Runを使用します  *//
                    //**************************************************************//
                    MessagePump.Run(form, form.MainLoop);
                }
                else
                {
                    //*********************************************************//
                    //* ここではメイン実装のメインフォームを起動します        *//
                    //*********************************************************//
                    var form = new MainForm();
                    //**************************************************************//
                    //* メインループを実行するために、MessagePump.Runを使用します  *//
                    //**************************************************************//
                    MessagePump.Run(form, form.MainLoop);
                }
            }
            else
            {
                using (var platform = new WindowsPlatform())
                {
                    // ImGuiベースのアプリケーションを起動する
                    platform.WindowOptions.Width = 1024;
                    platform.WindowOptions.Height = 768;
                    platform.WindowOptions.Title = "Rydia GameStudio - " + EditorContext.ProjectName;
                    platform.WindowOptions.RefreshMode = RefreshMode.VSync;
                    platform.WindowOptions.ScreenMode = ScreenMode.Window;

                    // オーディオ設定
                    // 既定のデバイスを使用
                    platform.AudioOptions.DeviceName = null;
                    // サンプリング周波数を44.1kHzに設定
                    platform.AudioOptions.Frequency = 44100;
                    // モノラル音源の数を16に設定
                    platform.AudioOptions.MonoSources = 16;
                    // ステレオ音源の数を16に設定
                    platform.AudioOptions.StereoSources = 16;
                    // 更新レートを60に設定
                    platform.AudioOptions.Refresh = 60;
                    // クロック同期を無効に設定
                    platform.AudioOptions.Sync = false;
                    platform.Run(new EditorApp());
                }
            }
        }
    }
}