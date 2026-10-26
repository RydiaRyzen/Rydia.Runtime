using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rydia.Input;

namespace Rydia.GameStudio.App
{

    /// <summary>
    /// WinForms の <see cref="Control"/> からキーボード入力を取得する入力ソースを表します。
    /// </summary>
    public class ControlKeyboardInputSource : IKeyboardInputSource, IDisposable
    {
        private readonly Control control;
        private bool hasFocus;

        private string charInput = string.Empty;
        private readonly StringBuilder charInputBuffer = new StringBuilder();

        private HashSet<Key> keyState = new HashSet<Key>();
        private readonly HashSet<Key> keyStateBuffer = new HashSet<Key>();

        /// <summary>
        /// 入力ソースを識別するための ID を取得します。
        /// </summary>
        public string Id
        {
            get { return "Keyboard"; }
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
            get { return "Keyboard"; }
        }

        /// <summary>
        /// キーボード入力が現在利用可能かどうかを示す値を取得します。
        /// </summary>
        public bool IsAvailable
        {
            get
            {
                return !this.control.IsDisposed &&
                       this.control.ContainsFocus;
            }
        }

        /// <summary>
        /// 現在のフレームで入力された文字列を取得します。
        /// </summary>
        public string CharInput
        {
            get { return this.charInput ?? string.Empty; }
        }

        /// <summary>
        /// 指定したキーが現在押されているかどうかを取得します。
        /// </summary>
        /// <param name="key">状態を取得するキー。</param>
        /// <returns>キーが押されている場合は true。</returns>
        public bool this[Key key]
        {
            get { return this.keyState.Contains(key); }
        }

        /// <summary>
        /// ControlKeyboardInputSource クラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="control">キーボード入力を取得するコントロール。</param>
        public ControlKeyboardInputSource(Control control)
        {
            if (control == null)
                throw new ArgumentNullException(nameof(control));

            this.control = control;

            this.control.KeyDown += HandleControlKeyDown;
            this.control.KeyUp += HandleControlKeyUp;
            this.control.KeyPress += HandleControlKeyPress;
            this.control.LostFocus += HandleControlLostFocus;
            this.control.Disposed += HandleControlDisposed;
        }

        /// <summary>
        /// キーが押されたときに呼び出されます。
        /// </summary>
        private void HandleControlKeyDown(object sender, KeyEventArgs e)
        {
            Key key = GetRydiaKey(e.KeyCode);

            if (key != Key.Unknown)
                this.keyStateBuffer.Add(key);
        }

        /// <summary>
        /// キーが離されたときに呼び出されます。
        /// </summary>
        private void HandleControlKeyUp(object sender, KeyEventArgs e)
        {
            Key key = GetRydiaKey(e.KeyCode);

            if (key != Key.Unknown)
                this.keyStateBuffer.Remove(key);
        }

        /// <summary>
        /// キー入力によって文字が入力されたときに呼び出されます。
        /// </summary>
        private void HandleControlKeyPress(
            object sender,
            KeyPressEventArgs e)
        {
            this.charInputBuffer.Append(e.KeyChar);
        }

        /// <summary>
        /// コントロールがフォーカスを失ったときに呼び出されます。
        /// </summary>
        private void HandleControlLostFocus(object sender, EventArgs e)
        {
            this.hasFocus = false;
            this.keyState.Clear();
            this.keyStateBuffer.Clear();
            this.charInputBuffer.Clear();
            this.charInput = string.Empty;
        }

        /// <summary>
        /// コントロールが破棄されたときに呼び出されます。
        /// </summary>
        private void HandleControlDisposed(object sender, EventArgs e)
        {
            Dispose();
        }

        /// <summary>
        /// 入力状態を更新します。
        /// </summary>
        /// <remarks>
        /// 文字入力とキー状態を確定します。
        /// フォーカスを失っている場合はキー状態をクリアします。
        /// </remarks>
        public void UpdateState()
        {
            this.hasFocus = this.control.ContainsFocus;

            if (!this.hasFocus)
            {
                this.keyState.Clear();
                this.keyStateBuffer.Clear();
                this.charInputBuffer.Clear();
                this.charInput = string.Empty;
                return;
            }

            this.charInput = this.charInputBuffer.ToString();
            this.charInputBuffer.Clear();

            this.keyState = new HashSet<Key>(this.keyStateBuffer);
        }

        /// <summary>
        /// WinForms のキーコードを Rydia のキーコードに変換します。
        /// </summary>
        /// <param name="key">変換する WinForms のキーコード。</param>
        /// <returns>対応する Rydia のキーコード。</returns>
        private static Key GetRydiaKey(Keys key)
        {
            switch (key)
            {
                case Keys.ShiftKey:
                case Keys.LShiftKey:
                    return Key.ShiftLeft;

                case Keys.RShiftKey:
                    return Key.ShiftRight;

                case Keys.ControlKey:
                case Keys.LControlKey:
                    return Key.ControlLeft;

                case Keys.RControlKey:
                    return Key.ControlRight;

                case Keys.Menu:
                case Keys.LMenu:
                    return Key.AltLeft;

                case Keys.RMenu:
                    return Key.AltRight;

                case Keys.LWin:
                    return Key.WinLeft;

                case Keys.RWin:
                    return Key.WinRight;

                case Keys.Apps:
                    return Key.Menu;

                case Keys.Return:
                    return Key.Enter;

                case Keys.Back:
                    return Key.BackSpace;

                case Keys.Prior:
                    return Key.PageUp;

                case Keys.Next:
                    return Key.PageDown;

                case Keys.Oemtilde:
                    return Key.Tilde;

                case Keys.OemMinus:
                    return Key.Minus;

                case Keys.Oemplus:
                    return Key.Plus;

                case Keys.OemOpenBrackets:
                    return Key.BracketLeft;

                case Keys.OemCloseBrackets:
                    return Key.BracketRight;

                case Keys.Oem1:
                    return Key.Semicolon;

                case Keys.Oem7:
                    return Key.Quote;

                case Keys.Oemcomma:
                    return Key.Comma;

                case Keys.OemPeriod:
                    return Key.Period;

                case Keys.OemQuestion:
                    return Key.Slash;

                case Keys.OemPipe:
                    return Key.BackSlash;

                case Keys.OemBackslash:
                    return Key.NonUSBackSlash;
            }

            // A～Z
            if (key >= Keys.A && key <= Keys.Z)
            {
                return (Key)Enum.Parse(
                    typeof(Key),
                    key.ToString());
            }

            // F1～F24
            if (key >= Keys.F1 && key <= Keys.F24)
            {
                return (Key)Enum.Parse(
                    typeof(Key),
                    key.ToString());
            }

            // 数字キー
            if (key >= Keys.D0 && key <= Keys.D9)
            {
                int number = (int)key - (int)Keys.D0;

                return (Key)Enum.Parse(
                    typeof(Key),
                    "Number" + number);
            }

            // テンキー
            if (key >= Keys.NumPad0 && key <= Keys.NumPad9)
            {
                int number = (int)key - (int)Keys.NumPad0;

                return (Key)Enum.Parse(
                    typeof(Key),
                    "Keypad" + number);
            }

            // 同名のキーを変換
            Key result;

            if (Enum.TryParse<Key>(key.ToString(), out result))
                return result;

            switch (key)
            {
                case Keys.Multiply:
                    return Key.KeypadMultiply;

                case Keys.Add:
                    return Key.KeypadAdd;

                case Keys.Subtract:
                    return Key.KeypadSubtract;

                case Keys.Divide:
                    return Key.KeypadDivide;

                case Keys.Decimal:
                    return Key.KeypadDecimal;
            }

            return Key.Unknown;
        }

        /// <summary>
        /// イベントの購読を解除します。
        /// </summary>
        public void Dispose()
        {
            this.control.KeyDown -= HandleControlKeyDown;
            this.control.KeyUp -= HandleControlKeyUp;
            this.control.KeyPress -= HandleControlKeyPress;
            this.control.LostFocus -= HandleControlLostFocus;
            this.control.Disposed -= HandleControlDisposed;

            this.keyState.Clear();
            this.keyStateBuffer.Clear();
            this.charInputBuffer.Clear();
            this.charInput = string.Empty;
            this.hasFocus = false;
        }
    }

}
