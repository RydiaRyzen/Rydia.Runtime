using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rydia.GameStudio
{

    public sealed class HierarchyView : EditorDockContent
    {
        private readonly TreeView treeView;

        public event Action<SceneObject> ObjectSelected;

        public HierarchyView()
            : base("Hierarchy")
        {
            this.treeView = new TreeView
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor,
                HideSelection = false
            };

            Controls.Add(this.treeView);

            var root = this.treeView.Nodes.Add("SampleScene");

            root.Nodes.Add(new SceneObjectNode(
                new SceneObject("Main Camera")));

            root.Nodes.Add(new SceneObjectNode(
                new SceneObject("Directional Light")));

            root.Nodes.Add(new SceneObjectNode(
                new SceneObject("Cube")));

            root.Expand();

            this.treeView.AfterSelect += (_, e) =>
            {
                if (e.Node is SceneObjectNode node)
                    ObjectSelected?.Invoke(node.Value);
            };
        }

        private sealed class SceneObjectNode : TreeNode
        {
            public SceneObject Value { get; }

            public SceneObjectNode(SceneObject value)
                : base(value.Name)
            {
                Value = value;
            }
        }
    }

}
