using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.GameStudio
{

    public sealed class InspectorView : EditorDockContent
    {
        private readonly PropertyGrid propertyGrid;

        public InspectorView()
            : base("Inspector")
        {
            this.propertyGrid = new PropertyGrid
            {
                Dock = DockStyle.Fill,
                ToolbarVisible = false,
                HelpVisible = true,
                BackColor = BackColor,
                ForeColor = ForeColor,
                ViewBackColor = BackColor,
                ViewForeColor = ForeColor,
                LineColor = Color.FromArgb(60, 64, 73)
            };

            Controls.Add(this.propertyGrid);
        }

        public void SetObject(SceneObject obj)
        {
            this.propertyGrid.SelectedObject = obj;
        }
    }

}
