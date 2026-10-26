using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rydia.Input;

namespace Rydia.GameStudio.App
{

    /// <summary>
    /// WinForms の <see cref="Control"/> からマウス入力を取得する入力ソースを表します。
    /// </summary>
    public sealed class ControlMouseInputSource : IMouseInputSource, IDisposable
    {
        /// <summary>
        /// カーソル位置を設定するためのデリゲートを表します。
        /// </summary>
        /// <param name="v">設定する座標値。</param>
        public delegate void CursorPosSetter(int v);

        private readonly Control control;

        private bool cursorInView;
        private Point2 position;

        private float wheel;
        private float wheelBuffer;

        private readonly HashSet<MouseButton> mouseState =
            new HashSet<MouseButton>();

        private readonly HashSet<MouseButton> mouseStateBuffer =
            new HashSet<MouseButton>();

        /// <summary>
        /// 入力ソースを識別するための ID を取得します。
        /// </summary>
        public string Id
        {
            get { return "Mouse"; }
        }

        /// <summary>
        /// 入力デバイスの製品 ID を取得します。
        /// </summary>
        public Guid ProductId
        {
            get { return Guid.Empty; }
        }

        /// <summary>
        /// 入力デバイスの製品名を取得します。
        /// </summary>
        public string ProductName
        {
            get { return "Mouse"; }
        }

        /// <summary>
        /// マウス入力が現在利用可能かどうかを示す値を取得します。
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return !this.control.IsDisposed &&
                       this.control.IsHandleCreated &&
                       this.cursorInView;
            }
        }

        /// <summary>
        /// マウスカーソルの位置を取得または設定します。
        /// </summary>
        /// <remarks>
        /// 取得時はコントロール内のクライアント座標を返します。
        /// 設定時は指定したクライアント座標をスクリーン座標に変換して
        /// カーソルを移動します。
        /// </remarks>
        public Point2 Pos
        {
            get { return this.position; }
            set
            {
                if (this.control.IsDisposed ||
                    !this.control.IsHandleCreated)
                {
                    return;
                }

                Point screenPoint = this.control.PointToScreen(
                    new Point(value.X, value.Y));

                Cursor.Position = screenPoint;
                this.position = value;
            }
        }

        /// <summary>
        /// マウスホイールの現在の値を取得します。
        /// </summary>
        public float Wheel
        {
            get { return this.wheel; }
        }

        /// <summary>
        /// 指定したマウスボタンが押されているかどうかを取得します。
        /// </summary>
        /// <param name="key">状態を取得するマウスボタン。</param>
        /// <returns>
        /// 指定したマウスボタンが押されている場合は
        /// <see langword="true"/>、それ以外の場合は
        /// <see langword="false"/>。
        /// </returns>
        public bool this[MouseButton key]
        {
            get { return this.mouseState.Contains(key); }
        }

        /// <summary>
        /// <see cref="ControlMouseInputSource"/> クラスの新しい
        /// インスタンスを初期化します。
        /// </summary>
        /// <param name="control">マウス入力を取得するコントロール。</param>
        public ControlMouseInputSource(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            this.control = control;

            this.control.MouseEnter += Control_MouseEnter;
            this.control.MouseLeave += Control_MouseLeave;
            this.control.MouseMove += Control_MouseMove;
            this.control.MouseWheel += Control_MouseWheel;
            this.control.MouseDown += Control_MouseDown;
            this.control.MouseUp += Control_MouseUp;
            this.control.MouseCaptureChanged += Control_MouseCaptureChanged;
            this.control.Disposed += Control_Disposed;
        }

        /// <summary>
        /// マウス入力の状態を更新します。
        /// </summary>
        /// <remarks>
        /// マウスイベントから取得した入力状態を現在の状態に反映します。
        /// </remarks>
        public void UpdateState()
        {
            if (this.control.IsDisposed)
                return;

            this.position = new Point2(
                this.control.PointToClient(Cursor.Position).X,
                this.control.PointToClient(Cursor.Position).Y);

            this.mouseState.Clear();
            foreach (MouseButton button in this.mouseStateBuffer)
                this.mouseState.Add(button);

            this.wheel = this.wheelBuffer;
        }

        /// <summary>
        /// マウスカーソルがコントロール内へ入ったときに呼び出されます。
        /// </summary>
        private void Control_MouseEnter(object sender, EventArgs e)
        {
            this.cursorInView = true;
        }

        /// <summary>
        /// マウスカーソルがコントロール外へ出たときに呼び出されます。
        /// </summary>
        private void Control_MouseLeave(object sender, EventArgs e)
        {
            this.cursorInView = false;
        }

        /// <summary>
        /// マウスカーソルが移動したときに呼び出されます。
        /// </summary>
        private void Control_MouseMove(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            this.position = new Point2(e.X, e.Y);
        }

        /// <summary>
        /// マウスホイールが操作されたときに呼び出されます。
        /// </summary>
        private void Control_MouseWheel(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            // WinForms の Delta は通常 120 単位で 1 ノッチです。
            this.wheelBuffer += e.Delta / 120.0f;
        }

        /// <summary>
        /// マウスボタンが押されたときに呼び出されます。
        /// </summary>
        private void Control_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseButton? button = GetRydiaMouseButton(e.Button);

            if (button.HasValue)
                this.mouseStateBuffer.Add(button.Value);
        }

        /// <summary>
        /// マウスボタンが離されたときに呼び出されます。
        /// </summary>
        private void Control_MouseUp(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            MouseButton? button = GetRydiaMouseButton(e.Button);

            if (button.HasValue)
                this.mouseStateBuffer.Remove(button.Value);
        }

        /// <summary>
        /// マウスキャプチャが変更されたときに呼び出されます。
        /// </summary>
        private void Control_MouseCaptureChanged(object sender, EventArgs e)
        {
            if (!this.control.Capture)
            {
                // ドラッグ中にコントロール外でボタンを離した場合などに、
                // ボタン状態が残らないようにします。
                this.mouseStateBuffer.Clear();
            }
        }

        /// <summary>
        /// コントロールが破棄されたときに呼び出されます。
        /// </summary>
        private void Control_Disposed(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// WinForms のマウスボタンを Rydia のマウスボタンに変換します。
        /// </summary>
        /// <param name="button">変換する WinForms のマウスボタン。</param>
        /// <returns>対応する Rydia のマウスボタン。</returns>
        private static MouseButton? GetRydiaMouseButton(MouseButtons button)
        {
            switch (button)
            {
                case MouseButtons.Left:
                    return MouseButton.Left;

                case MouseButtons.Right:
                    return MouseButton.Right;

                case MouseButtons.Middle:
                    return MouseButton.Middle;

                case MouseButtons.XButton1:
                    return MouseButton.Extra1;

                case MouseButtons.XButton2:
                    return MouseButton.Extra2;

                default:
                    return null;
            }
        }

        /// <summary>
        /// イベントの購読を解除し、入力状態をクリアします。
        /// </summary>
        public void Dispose()
        {
            this.control.MouseEnter -= Control_MouseEnter;
            this.control.MouseLeave -= Control_MouseLeave;
            this.control.MouseMove -= Control_MouseMove;
            this.control.MouseWheel -= Control_MouseWheel;
            this.control.MouseDown -= Control_MouseDown;
            this.control.MouseUp -= Control_MouseUp;
            this.control.MouseCaptureChanged -= Control_MouseCaptureChanged;
            this.control.Disposed -= Control_Disposed;

            this.mouseState.Clear();
            this.mouseStateBuffer.Clear();
            this.cursorInView = false;
        }
    }

}
