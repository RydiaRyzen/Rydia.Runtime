using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;
using Aga.Controls.Tree;
using Aga.Controls.Tree.NodeControls;
using Rydia.Engine;
using Rydia.Engine.Serialization;
using static System.Net.Mime.MediaTypeNames;

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
        private readonly string mediaDirectory;

        /// <summary>
        /// 新しい ProjectView を初期化します。
        /// </summary>
        public ProjectView()
            : base("Project")
        {
            this.assetsDirectory = EditorContext.AssetsPath;
            this.mediaDirectory = EditorContext.MediaPath;
            this.treeModel = new TreeModel();

            this.treeView = new TreeViewAdv
            {
                Dock = DockStyle.Fill,
                BorderStyle = BorderStyle.None,
                BackColor = BackColor,
                ForeColor = ForeColor,
                Model = this.treeModel,
                // ドラッグ＆ドロップを有効化
                AllowDrop = true
            };

            // ドラッグ＆ドロップイベント
            this.treeView.DragEnter += TreeView_DragEnter;
            this.treeView.DragDrop += TreeView_DragDrop;
            this.treeView.ItemDrag += TreeView_ItemDrag;

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

        /// <summary>
        /// ノードのドラッグを開始します。
        /// </summary>
        private void TreeView_ItemDrag(object? sender, ItemDragEventArgs e)
        {
            Debug.WriteLine($"Item Type: {e.Item?.GetType().FullName}");
            Debug.WriteLine($"Item Value: {e.Item}");
            if (e.Item != null)
            {
                foreach (PropertyInfo property in e.Item.GetType().GetProperties())
                {
                    try
                    {
                        Debug.WriteLine(
                            $"{property.Name}: {property.GetValue(e.Item)}");
                    }
                    catch
                    {
                    }
                }
            }
            if (e.Item is not TreeNodeAdv[] nodes || nodes.Length == 0)
                return;
            TreeNodeAdv node = nodes[0];

            if (node.Tag is not Node model)
                return;

            if (model.Tag is not string path || !File.Exists(path))
                return;

            this.treeView.DoDragDrop(
                path,
                DragDropEffects.Move);
        }

        /// <summary>
        /// ファイルがドラッグされたときの処理を行います。
        /// </summary>
        private void TreeView_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data == null)
                return;
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
            }
            else if (e.Data.GetDataPresent(typeof(string)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        /// <summary>
        /// ファイルがドロップされたときの処理を行います。
        /// </summary>
        private void TreeView_DragDrop(object? sender, DragEventArgs e)
        {
            if (e.Data == null)
                return;

            // ドロップ位置のノードを取得
            Point clientPoint = this.treeView.PointToClient(new Point(e.X, e.Y));
            TreeNodeAdv? treeNode = this.treeView.GetNodeAt(clientPoint);

            // ノードが取得できない場合はAssetsルートを対象にする
            Node? targetNode = treeNode?.Tag as Node;

            string targetDirectory = targetNode?.Tag as string ?? this.assetsDirectory;
            // ファイルノードにドロップされた場合は親ディレクトリを対象にする
            if (File.Exists(targetDirectory))
            {
                targetDirectory = Path.GetDirectoryName(targetDirectory)
                    ?? this.assetsDirectory;
            }

            // ディレクトリを作成
            Directory.CreateDirectory(this.mediaDirectory);
            Directory.CreateDirectory(targetDirectory);

            // Assets内のファイル移動
            if (e.Data.GetDataPresent(typeof(string)))
            {
                if (e.Data.GetData(typeof(string)) is string sourcePath)
                {
                    MoveAsset(sourcePath, targetDirectory);
                }
            }
            // 外部ファイルのインポート
            else if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                if (e.Data.GetData(DataFormats.FileDrop) is string[] files)
                {
                    ImportExternalFiles(files, targetDirectory);
                }
            }

            // Assets ビューを更新
            RefreshAssets();
            this.treeView.ExpandAll();
        }

        /// <summary>
        /// 外部ファイルをMediaにコピーしてインポートします。
        /// </summary>
        private void ImportExternalFiles(
            string[] files,
            string targetDirectory)
        {
            foreach (string file in files)
            {
                if (!File.Exists(file))
                {
                    continue;
                }

                string fileName = Path.GetFileName(file);
                string destination = Path.Combine(this.mediaDirectory, fileName);

                // 同名ファイルが存在する場合は連番を付ける
                destination = GetUniqueFilePath(destination);

                try
                {
                    // Mediaにコピー
                    File.Copy(file, destination);
                    // インポート
                    ImportAsset(destination, targetDirectory);
                }
                catch (IOException ex)
                {
                    MessageBox.Show(
                        $"ファイルのコピーに失敗しました。\n{file}\n\n{ex.Message}",
                        "ファイルコピーエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (UnauthorizedAccessException ex)
                {
                    MessageBox.Show(
                        $"ファイルへのアクセスに失敗しました。\n{file}\n\n{ex.Message}",
                        "アクセスエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(
                        $"ファイルのインポートに失敗しました。\n{file}\n\n{ex.Message}",
                        "インポートエラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
            }
        }

        /// <summary>
        /// Assets内のファイルを指定されたディレクトリに移動します。
        /// </summary>
        /// <param name="sourcePath">移動元のファイルパス。</param>
        /// <param name="targetDirectory">移動先のディレクトリ。</param>
        private void MoveAsset(string sourcePath, string targetDirectory)
        {
            if (!File.Exists(sourcePath))
                return;

            string fullAssetsPath = Path.GetFullPath(this.assetsDirectory)
                .TrimEnd(Path.DirectorySeparatorChar) +
                Path.DirectorySeparatorChar;

            string fullSourcePath = Path.GetFullPath(sourcePath);
            string fullTargetDirectory = Path.GetFullPath(targetDirectory);

            // Assets外のファイルは移動しない
            if (!fullSourcePath.StartsWith(
                fullAssetsPath,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // Assets外への移動を禁止
            if (!fullTargetDirectory.StartsWith(
                fullAssetsPath,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            // 同じディレクトリなら何もしない
            if (string.Equals(
                Path.GetDirectoryName(fullSourcePath),
                fullTargetDirectory,
                StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            string destination = Path.Combine(
                fullTargetDirectory,
                Path.GetFileName(fullSourcePath));

            // 同名ファイルが存在する場合は移動しない
            if (File.Exists(destination))
            {
                MessageBox.Show(
                    "移動先に同名のファイルが存在します。",
                    "ファイル移動",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                return;
            }

            File.Move(fullSourcePath, destination);

        }

        /// <summary>
        /// 指定されたファイルをアセットとしてインポートします。
        /// </summary>
        /// <param name="sourcePath">インポート元のファイルパス。</param>
        /// <param name="targetDirectory">インポート先のディレクトリ。</param>
        private static void ImportAsset(
            string sourcePath,
            string targetDirectory)
        {

            string ext = Path.GetExtension(sourcePath).ToLowerInvariant();

            string targetFileName =
                Path.GetFileNameWithoutExtension(sourcePath) + ".ryd";

            string assetFilePath = Path.Combine(
                targetDirectory,
                targetFileName);

            var pixelDataSerializer = new XmlResourceSerializer<PixelData>();
            var audioDataSerializer = new XmlResourceSerializer<AudioData>();
            if (ext == ".jpeg" || ext == ".jpg")
            {
                var importer = new Assets.JpegImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out PixelData pixdata))
                {
                    pixdata.Path = assetFilePath;
                    pixelDataSerializer.Serialize(pixdata, assetFilePath);
                }
            }
            else if (ext == ".png")
            {
                var importer = new Assets.JpegImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out PixelData pixdata))
                {
                    pixdata.Path = assetFilePath;
                    pixelDataSerializer.Serialize(pixdata, assetFilePath);
                }
            }
            else if (ext == ".bmp")
            {
                var importer = new Assets.JpegImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out PixelData pixdata))
                {
                    pixdata.Path = assetFilePath;
                    pixelDataSerializer.Serialize(pixdata, assetFilePath);
                }
            }
            else if (ext == ".tga")
            {
                var importer = new Assets.TgaImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out PixelData pixdata))
                {
                    pixdata.Path = assetFilePath;
                    pixelDataSerializer.Serialize(pixdata, assetFilePath);
                }
            }
            else if (ext == ".wav")
            {
                var importer = new Assets.WavImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out AudioData audioData))
                {
                    audioData.Path = assetFilePath;
                    audioDataSerializer.Serialize(audioData, assetFilePath);
                }
            }
            else if (ext == ".ogg")
            {
                var importer = new Assets.OggImporter();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out AudioData audioData))
                {
                    audioData.Path = assetFilePath;
                    audioDataSerializer.Serialize(audioData, assetFilePath);
                }
            }
            else if (ext == ".mp3")
            {
                var importer = new Assets.Mp3Importer();

                if (importer.TryImport(
                    File.ReadAllBytes(sourcePath),
                    out AudioData audioData))
                {
                    audioData.Path = assetFilePath;
                    audioDataSerializer.Serialize(audioData, assetFilePath);
                }
            }
        }

        /// <summary>
        /// 同名ファイルが存在しないファイルパスを取得します。
        /// </summary>
        /// <param name="filePath">元のファイルパス。</param>
        /// <returns>重複しないファイルパス。</returns>
        private static string GetUniqueFilePath(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return filePath;
            }

            string directory = Path.GetDirectoryName(filePath)!;
            string name = Path.GetFileNameWithoutExtension(filePath);
            string extension = Path.GetExtension(filePath);

            int index = 1;

            while (true)
            {
                string newFileName = $"{name}_{index}{extension}";
                string newFilePath = Path.Combine(directory, newFileName);

                if (!File.Exists(newFilePath))
                {
                    return newFilePath;
                }

                index++;
            }
        }

    }

}
