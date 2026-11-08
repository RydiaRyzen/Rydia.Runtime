using System;
using System.Collections.Generic;
using System.Linq;
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
        /// ファイルがドラッグされたときの処理を行います。
        /// </summary>
        private void TreeView_DragEnter(object? sender, DragEventArgs e)
        {
            if (e.Data != null &&
                e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
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
            if (e.Data == null ||
                !e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                return;
            }

            if (e.Data.GetData(DataFormats.FileDrop) is not string[] files)
            {
                return;
            }

            // Media ディレクトリを作成
            Directory.CreateDirectory(this.mediaDirectory);

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
                    File.Copy(file, destination);
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
                        $"予期せぬエラーが発生しました。\n{file}\n\n{ex.Message}",
                        "エラー",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
                }
                // インポート操作
                var ext = Path.GetExtension(file);
                var targetFilename = Path.GetFileNameWithoutExtension(destination) + ".ryd";
                var assetsFilepPath = Path.Combine("Assets", Path.Combine(EditorContext.AssetsPath, targetFilename));
                if (ext == ".jpeg" || ext == ".jpg")
                {
                    var importer = new Assets.JpegImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out PixelData pixdata);
                    pixdata.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<PixelData>();
                    serializer.Serialize(pixdata, assetsFilepPath);
                }
                else if (ext == ".png")
                {
                    var importer = new Assets.JpegImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out PixelData pixdata);
                    pixdata.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<PixelData>();
                    serializer.Serialize(pixdata, assetsFilepPath);
                }
                else if (ext == ".bmp")
                {
                    var importer = new Assets.JpegImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out PixelData pixdata);
                    pixdata.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<PixelData>();
                    serializer.Serialize(pixdata, assetsFilepPath);
                }
                else if (ext == ".tga")
                {
                    var importer = new Assets.TgaImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out PixelData pixdata);
                    pixdata.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<PixelData>();
                    serializer.Serialize(pixdata, assetsFilepPath);
                }
                else if (ext == ".wav")
                {
                    var importer = new Assets.WavImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out AudioData audioData);
                    audioData.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<AudioData>();
                    serializer.Serialize(audioData, assetsFilepPath);
                }
                else if (ext == ".ogg")
                {
                    var importer = new Assets.OggImporter();
                    importer.TryImport(File.ReadAllBytes(destination), out AudioData audioData);
                    audioData.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<AudioData>();
                    serializer.Serialize(audioData, assetsFilepPath);
                }
                else if (ext == ".mp3")
                {
                    var importer = new Assets.Mp3Importer();
                    importer.TryImport(File.ReadAllBytes(destination), out AudioData audioData);
                    audioData.Path = assetsFilepPath;
                    var serializer = new XmlResourceSerializer<AudioData>();
                    serializer.Serialize(audioData, assetsFilepPath);
                }
            }

            

            // Assets ビューを更新
            RefreshAssets();
            this.treeView.ExpandAll();
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
