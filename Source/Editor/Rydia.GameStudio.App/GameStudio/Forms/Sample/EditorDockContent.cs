using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;
using WeifenLuo.WinFormsUI.Docking;

namespace Rydia.GameStudio
{

    public abstract class EditorDockContent : DockContent
    {
        protected EditorDockContent(string title)
        {
            Text = title;
            BackColor = Color.FromArgb(38, 41, 49);
            ForeColor = Color.White;
            ShowHint = DockState.Document;
        }

        protected Control CreatePanel()
        {
            return new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(38, 41, 49)
            };
        }
    }

}
