using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.GameStudio
{

    public sealed class ProjectView : EditorDockContent
    {
        private readonly TreeView treeView;

        public ProjectView()
            : base("Project")
        {
            this.treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor
            };

            Controls.Add(this.treeView);

            var root = this.treeView.Nodes.Add("Assets");

            foreach (string directory in new[]
            {
                "Scenes", "Materials", "Textures", "Scripts"
            })
            {
                root.Nodes.Add(directory);
            }

            root.Expand();
        }
    }

}
