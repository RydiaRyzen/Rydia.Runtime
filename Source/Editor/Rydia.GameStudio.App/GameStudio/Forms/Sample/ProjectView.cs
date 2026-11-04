using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;

namespace Rydia.GameStudio
{

    /// <summary>
    /// プロジェクト内のアセットを表示するビューを表します。
    /// </summary>
    public sealed class ProjectView : EditorDockContent
    {
        private readonly Aga.Controls.Tree.TreeViewAdv treeView;
        private readonly TreeModel treeModel;
        private readonly string assetsDirectory;

        /// <summary>
        /// 新しい ProjectView を初期化します。
        /// </summary>
        public ProjectView()
            : base("Project")
        {
            this.assetsDirectory = EditorContext.AssetsPath;

            this.treeModel = new TreeModel();

            this.treeView = new TreeViewAdv
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor,
                Model = this.treeModel
            };

            // ノードのテキスト表示設定
            TreeColumn column = new TreeColumn();

            this.treeView.Columns.Add(column);

            NodeTextBox textBox = new NodeTextBox
            {
                DataPropertyName = "Text",
                ParentColumn = column
            };

            this.treeView.NodeControls.Add(textBox);

            Controls.Add(this.treeView);

            // Assets 以下のファイルを読み込む
            RefreshAssets();
            this.treeView.ExpandAll();
        }

        /// <summary>
        /// Assets フォルダ以下のディレクトリとファイルを再読み込みします。
        /// </summary>
        public void RefreshAssets()
        {
            this.treeModel.Nodes.Clear();

            if (!Directory.Exists(this.assetsDirectory))
            {
                Directory.CreateDirectory(this.assetsDirectory);
            }

            // Assets ルートノード
            Node root = new Node("Assets")
            {
                Tag = this.assetsDirectory
            };
            root.Image = Rydia.GameStudio.Properties.EditorResource.folder;

            this.treeModel.Nodes.Add(root);

            // 子ディレクトリとファイルを追加
            AddDirectoryNodes(root, this.assetsDirectory);
        }

        /// <summary>
        /// 指定されたディレクトリ以下のノードを再帰的に追加します。
        /// </summary>
        /// <param name="parent">追加先の親ノード。</param>
        /// <param name="directoryPath">読み込むディレクトリのパス。</param>
        private static void AddDirectoryNodes(Node parent, string directoryPath)
        {
            // ディレクトリを追加
            foreach (string directory in Directory.GetDirectories(directoryPath))
            {
                Node node = new Node(Path.GetFileName(directory))
                {
                    Tag = directory
                };
                node.Image = Rydia.GameStudio.Properties.EditorResource.folder;
                parent.Nodes.Add(node);

                AddDirectoryNodes(node, directory);
            }

            // ファイルを追加
            foreach (string file in Directory.GetFiles(directoryPath))
            {
                Node node = new Node(Path.GetFileName(file))
                {
                    Tag = file
                };
                node.Image = Rydia.GameStudio.Properties.EditorResource.CursorArrow;
                parent.Nodes.Add(node);
            }
        }

    }

}
