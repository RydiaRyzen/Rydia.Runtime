using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Rydia.Win32
{
    public static class User32Interop
    {
        /// <summary>
        /// 着信した送信済みメッセージをディスパッチ（送出）し、
        /// スレッドのメッセージキューにポスト済みメッセージが存在するかどうかをチェックし、
        /// 存在する場合は、指定された構造体にそのメッセージを格納します。
        /// </summary>
        /// <param name="msg">
        /// 1 個の 構造体へのポインタを指定します。
        /// 関数から制御が返ると、この構造体に、メッセージ情報が格納されます。
        /// </param>
        /// <param name="hWnd">メッセージの取得に使う（取得元）ウィンドウのハンドルを指定します。</param>
        /// <param name="messageFilterMin">取得対象のメッセージの最小値を整数で指定します。</param>
        /// <param name="messageFilterMax">取得対象のメッセージの最大値を整数で指定します。</param>
        /// <param name="flags">メッセージの処理方法を指定します。
        /// </param>
        /// <returns>
        /// メッセージを取得した場合、trueが返ります。
        /// メッセージを取得しなかった場合、falseが返ります。
        /// </returns>
        [SuppressUnmanagedCodeSecurity] //for performance
        [DllImport("user32.dll")]
        public static extern bool PeekMessage(
        out Message msg,
        IntPtr hWnd,
        uint messageFilterMin,
        uint messageFilterMax,
        uint flags
        );

        /// <summary>
        /// ウィンドウのクライアント領域の左上端と右下端の座標を返します。
        /// 座標は、クライアント座標で返されます。
        /// クライアント座標はクライアント領域の左上端からの相対座標なので、左上端の座標は常に (0,0) となります。
        /// </summary>
        /// <param name="hWnd">クライアント領域の座標を取得するウィンドウのハンドルを指定します</param>
        /// <param name="rect">
        /// 構造体へのポインタを指定します。
        /// この構造体にクライアント領域の座標が格納されます。
        /// left メンバと top メンバは 0 になります。
        /// right メンバと bottom メンバは、クライアント領域の幅と高さになります。
        /// </param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、 関数を使います。
        /// </returns>
        [DllImport("User32.Dll")]
        public static extern bool GetClientRect(IntPtr hWnd, out RECT rect);

        /// <summary>
        /// 指定されたウィンドウの左上端と右下端の座標をスクリーン座標で取得します。
        /// スクリーン座標は、表示画面の左上端が (0,0) となります。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドルを指定します</param>
        /// <param name="rect">
        /// 構造体へのポインタを指定します。
        /// この構造体の left メンバと top メンバに、スクリーン座標でのウィンドウ左上端の座標が入ります。
        /// right メンバと bottom メンバには、ウィンドウの右下端の座標が入ります。
        /// </param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。拡張エラー情報を取得するには、 関数を使います。
        /// </returns>
        [DllImport("User32.Dll")]
        public static extern bool GetWindowRect(IntPtr hWnd, out RECT rect);

        /// <summary>
        /// 指定された点を、クライアント座標からスクリーン座標へ変換します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドルを指定します。このウィンドウのクライアント領域を利用して変換を行います。</param>
        /// <param name="lpPoint">
        /// 変換対象のクライアント座標を保持している、1 個の 構造体へのポインタを指定します。
        /// 関数から制御が返り、関数が成功すると、この構造体にスクリーン座標が格納されます。</param>
        /// <returns>
        /// 関数が成功すると、0 以外の値が返ります。
        /// 関数が失敗すると、0 が返ります。
        /// </returns>
        [DllImport("User32.Dll")]
        public static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);

        /// <summary>
        /// 表示されているウィンドウを指定したデバイス コンテキスト (通常はプリンター DC) にコピーします。
        /// </summary>
        /// <param name="hwnd">ウィンドウのハンドルを指定します。</param>
        /// <param name="hDC">出力されるデバイス コンテキストへのポインター</param>
        /// <param name="nFlags">描画のオプションを指定します。</param>
        /// <returns></returns>
        [DllImport("User32.dll")]
        public static extern bool PrintWindow(IntPtr hwnd, IntPtr hDC, PrintWindowCommand nFlags);
        /*
        /// <summary>
        /// 別のスレッドによって作成されたウィンドウの表示状態を設定します。
        /// </summary>
        /// <param name="hWnd">ウィンドウのハンドルを指定します。</param>
        /// <param name="nCmdShow">
        /// ウィンドウの表示方法を指定します。</param>
        /// <returns>
        /// 設定前にウィンドウが可視だった場合、0 以外の値が返ります。
        /// 設定前にウィンドウが隠れていた場合、0 が返ります。
        /// </returns>
        /// <remarks>
        /// この関数は、指定されたウィンドウのメッセージキューにウィンドウ表示イベントをポストします。
        /// ハング状態のアプリケーションがウィンドウ表示イベント処理の完了を待機している間、
        /// 自分のアプリケーションがハングするのを防ぐことができます。
        /// </remarks>
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(IntPtr hWnd, ShowWindowCommand nCmdShow);
        */
        /// <summary>
        /// 指定されたウィンドウが最小化（ アイコン化）されているかどうかを調べます。
        /// </summary>
        /// <param name="hWnd">調査するウィンドウのハンドルを指定します</param>
        /// <returns>
        /// ウィンドウが最小化されている場合はtrue, それ以外の場合はfalseを返します
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool IsIconic(IntPtr hWnd);

        /// <summary>
        /// 指定されたウィンドウの位置とサイズを変更します。
        /// トップレベルウィンドウ（ 親を持たないウィンドウ）の場合は、
        /// 画面左上端からの相対位置（ スクリーン座標）で位置とサイズを指定します。
        /// 子ウィンドウの場合は、親ウィンドウのクライアント領域の左上端からの相対位置（ クライアント座標）で指定します。
        /// </summary>
        /// <param name="hwnd">ウィンドウのハンドルを指定します。</param>
        /// <param name="x">ウィンドウの左端の新しい位置を指定します。</param>
        /// <param name="y">ウィンドウの上端の新しい位置を指定します。</param>
        /// <param name="nWidth">ウィンドウの新しい幅を指定します。</param>
        /// <param name="nHeight">ウィンドウの新しい高さを指定します。</param>
        /// <param name="bRepaint">
        /// ウィンドウを再描画するかどうかを指定します。
        /// TRUE を指定すると、ウィンドウに メッセージが送られます。
        /// FALSE を指定すると、再描画は行われません。
        /// この指定は、クライアント領域、非クライアント領域（ タイトルバーとスクロールバーを含む）、
        /// および親ウィンドウの、子ウィンドウを移動したことにより画面に現れたあらゆる部分に適用されます。
        /// </param>
        /// <returns></returns>
        [DllImport("user32.dll")]
        public static extern bool MoveWindow(IntPtr hwnd, int x, int y, int nWidth, int nHeight, int bRepaint);

        /// <summary>
        /// 指定されたウィンドウを作成したスレッドをフォアグラウンドにし、
        /// そのウィンドウをアクティブにします。
        /// キーボード入力はこのウィンドウに直接送られるようになり、
        /// ユーザーにこれがわかるように画面でのさまざまな表示が変更されます。
        /// Windows システムは、フォアグラウンドウィンドウを生成したスレッドに対して、
        /// 他のスレッドよりも若干高い優先順位を割り当てます。
        /// </summary>
        /// <param name="hWnd">アクティブにし、フォアグラウンドにするウィンドウのハンドルを指定します。</param>
        /// <returns>
        /// ウィンドウがフォアグラウンドになった場合はtrue,
        /// それ以外の場合はfalseを返します
        /// </returns>
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

    }

}
