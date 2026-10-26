using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.GameStudio.App
{

    /// <summary>
    /// アプリケーションメインループを実装します
    /// </summary>
    public static class MessagePump
    {

        /// <summary>
        /// メッセージが取得されなかったかどうかを取得します
        /// </summary>
        private static bool AppStillIdle
        {
            get
            {
                Message msg;
                return !Rydia.Win32.User32Interop.PeekMessage(out msg, IntPtr.Zero, 0, 0, 0);
            }
        }

        /// <summary>
        /// <see cref="Application.Idle"/>イベントにメインループ実行処理を追加します。
        /// </summary>
        /// <param name="mainLoop">mainLoop</param>
        private static void AddApplicationIdle(this ApplicationMainLoop mainLoop)
        {
            Application.Idle += delegate
            {
                while (AppStillIdle)
                {
                    mainLoop.Invoke();
                }
            };
        }

        /// <summary>
        /// 現在のスレッドで、標準のアプリケーションメッセージループを実行し、
        /// アイドル状態の際に指定された<see cref="ApplicationMainLoop"/>を実行する処理をフォーム無しで開始します
        /// </summary>
        /// <param name="mainLoop">実行するメインループ</param>
        public static void Run(ApplicationMainLoop mainLoop)
        {
            mainLoop.AddApplicationIdle();
            Application.Run();
        }

        /// <summary>
        /// 現在のスレッドで、標準のアプリケーションメッセージループを実行し、
        /// アイドル状態の際に指定された<see cref="ApplicationMainLoop"/>を実行する処理を指定したフォームを使用して開始します
        /// </summary>
        /// <param name="mainForm">mainForm</param>
        /// <param name="mainLoop">mainLoop</param>
        public static void Run(Form mainForm, ApplicationMainLoop mainLoop)
        {
            mainLoop.AddApplicationIdle();
            Application.Run(mainForm);
        }

        /// <summary>
        /// <see cref="ApplicationContext"/>を使用して、
        /// 現在のスレッドで標準のアプリケーションメッセージループを実行し、
        /// アイドル状態の際に指定された<see cref="ApplicationMainLoop"/>を実行する処理を開始します
        /// </summary>
        /// <param name="context">context</param>
        /// <param name="mainLoop">mainLoop</param>
        public static void Run(ApplicationContext context, ApplicationMainLoop mainLoop)
        {
            mainLoop.AddApplicationIdle();
            Application.Run(context);
        }

    }


}
