using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rydia.GameStudio.App;
using Rydia.IO;
using Rydia.SolutionBuilder;

namespace Rydia.GameStudio
{

    /// <summary>
    /// 新しいプロジェクトを作成するためのフォームを表します。
    /// </summary>
    public partial class NewProjectForm : Form
    {

        private TextBox projectNameTextBox;
        private ComboBox templateComboBox;
        private Button createButton;
        private Button cancelButton;

        /// <summary>
        /// 作成するプロジェクトの名前を取得します。
        /// </summary>
        public string ProjectName
        {
            get
            {
                return this.projectNameTextBox.Text.Trim();
            }
        }

        /// <summary>
        /// プロジェクトの保存先ディレクトリを取得します。
        /// </summary>
        public string ProjectLocation
        {
            get
            {
                return Path.Combine(Environment.CurrentDirectory, "Projects");
            }
        }

        /// <summary>
        /// 選択されたプロジェクトテンプレートを取得します。
        /// </summary>
        public string SelectedTemplate
        {
            get
            {
                return this.templateComboBox.SelectedItem?.ToString() ?? string.Empty;
            }
        }

        /// <summary>
        /// 新しいプロジェクト作成フォームを初期化します。
        /// </summary>
        public NewProjectForm()
        {
            InitializeComponent();

            var root = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 4,
                Padding = new Padding(24),
                BackColor = BackColor
            };

            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 60));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            root.RowStyles.Add(new RowStyle(SizeType.Absolute, 52));

            var heading = new Label
            {
                Text = "新しいプロジェクト",
                Dock = DockStyle.Fill,
                Font = new Font("Segoe UI", 16F, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft
            };

            var templatePanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 2,
                BackColor = BackColor
            };
            templatePanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 120));
            templatePanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));
            templatePanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32));
            templatePanel.RowStyles.Add(
                new RowStyle(SizeType.Percent, 100));

            var templateLabel = CreateLabel("テンプレート");

            this.templateComboBox = new ComboBox
            {
                Dock = DockStyle.Fill,
                DropDownStyle = ComboBoxStyle.DropDownList,
                BackColor = Color.FromArgb(55, 55, 58),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };

            this.templateComboBox.Items.AddRange(new object[]
            {
            "3D プロジェクト",
            "2D プロジェクト",
            });
            this.templateComboBox.SelectedIndex = 0;

            templatePanel.Controls.Add(templateLabel, 0, 0);
            templatePanel.SetColumnSpan(templateLabel, 2);
            templatePanel.Controls.Add(this.templateComboBox, 0, 1);
            templatePanel.SetColumnSpan(this.templateComboBox, 2);

            var settingsPanel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 4,
                BackColor = BackColor
            };
            settingsPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Absolute, 120));
            settingsPanel.ColumnStyles.Add(
                new ColumnStyle(SizeType.Percent, 100));

            settingsPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32));
            settingsPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40));
            settingsPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 32));
            settingsPanel.RowStyles.Add(
                new RowStyle(SizeType.Absolute, 40));

            var nameLabel = CreateLabel("プロジェクト名");

            this.projectNameTextBox = new TextBox
            {
                Text = "MyProject",
                Dock = DockStyle.Fill,
                BackColor = Color.FromArgb(55, 55, 58),
                ForeColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };
            settingsPanel.Controls.Add(nameLabel, 0, 0);
            settingsPanel.Controls.Add(this.projectNameTextBox, 0, 1);
            settingsPanel.SetColumnSpan(this.projectNameTextBox, 2);
            var buttonPanel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.RightToLeft,
                WrapContents = false,
                BackColor = BackColor,
                Padding = new Padding(0, 8, 0, 0)
            };

            this.createButton = new Button
            {
                Text = "作成",
                Width = 100,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(45, 110, 190),
                ForeColor = Color.White,
                DialogResult = DialogResult.None
            };
            this.createButton.FlatAppearance.BorderSize = 0;
            this.createButton.Click += createButton_Click;

            this.cancelButton = new Button
            {
                Text = "キャンセル",
                Width = 100,
                Height = 32,
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(63, 63, 65),
                ForeColor = Color.White,
                DialogResult = DialogResult.Cancel
            };
            this.cancelButton.FlatAppearance.BorderSize = 0;
            this.cancelButton.Click += (_, _) => Close();

            buttonPanel.Controls.Add(this.createButton);
            buttonPanel.Controls.Add(this.cancelButton);

            root.Controls.Add(heading, 0, 0);
            root.Controls.Add(templatePanel, 0, 1);
            root.Controls.Add(settingsPanel, 0, 2);
            root.Controls.Add(buttonPanel, 0, 3);

            Controls.Add(root);

            AcceptButton = this.createButton;
            CancelButton = this.cancelButton;

        }

        /// <summary>
        /// 指定された文字列を表示するラベルを生成します。
        /// </summary>
        /// <param name="text">ラベルに表示する文字列。</param>
        /// <returns>生成されたラベル。</returns>
        private static Label CreateLabel(string text)
        {
            return new Label
            {
                Text = text,
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                ForeColor = Color.Gainsboro
            };
        }

        /// <summary>
        /// 入力内容を検証し、新しいプロジェクトの作成を確定します。
        /// </summary>
        private async void createButton_Click(object sender, EventArgs e)
        {
            string projectName = ProjectName;
            string projectLocation = ProjectLocation;

            if (string.IsNullOrWhiteSpace(projectName))
            {
                MessageBox.Show(
                    this,
                    "プロジェクト名を入力してください。",
                    "入力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.projectNameTextBox.Focus();
                return;
            }

            if (projectName.IndexOfAny(Path.GetInvalidFileNameChars()) >= 0 ||
                projectName == "." || projectName == "..")
            {
                MessageBox.Show(
                    this,
                    "プロジェクト名に使用できない文字が含まれています。",
                    "入力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                this.projectNameTextBox.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(projectLocation))
            {
                MessageBox.Show(
                    this,
                    "保存場所を指定してください。",
                    "入力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                //this.locationTextBox.Focus();
                return;
            }

            try
            {
                // 保存先として指定されたパスが有効か確認します。
                _ = Path.GetFullPath(projectLocation);
            }
            catch (Exception ex) when (
                ex is ArgumentException ||
                ex is NotSupportedException ||
                ex is PathTooLongException)
            {
                MessageBox.Show(
                    this,
                    "保存場所が正しくありません。",
                    "入力エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);

                //this.locationTextBox.Focus();
                return;
            }

            // ログ表示フォームを開く
            using var logForm = new ProjectGenerationLogForm();
            logForm.Show(this);
            logForm.AppendTitle("プロジェクト生成");
            logForm.AppendLog("プロジェクト生成処理を開始します。");

            this.createButton.Enabled = false;

            bool success = false;


            // TODO : プロジェクト作成
            // プロジェクトの保存先ディレクトリを作成
            var editordirectory = Path.GetDirectoryName(projectLocation);
            var projectDirectory = Path.Combine(projectLocation, projectName);
            Directory.CreateDirectory(projectDirectory);
            var assetsDirectory = Path.Combine(projectDirectory, "Assets");
            Directory.CreateDirectory(assetsDirectory);
            var mediaDirectory = Path.Combine(projectDirectory, "Source", "Media");
            Directory.CreateDirectory(mediaDirectory);
            var sourceDirectory = Path.Combine(projectDirectory, "Source", "Code");
            Directory.CreateDirectory(sourceDirectory);
            // ソリューションを生成
            var solutionFile = new SolutionFile(projectName);
            try
            {
                // ファイル生成とビルドを UI スレッドから分離する
                await Task.Run(() =>
                {
                    logForm.SetStatus("ソリューションを生成しています...");
                    logForm.AppendLog("ソリューション生成を開始します。");

                    // ソリューションのパスを設定
                    solutionFile.Path = Path.Combine(sourceDirectory);

                    // プロジェクトC: 共有プロジェクトを生成
                    var projectC = new ProjectFile(projectName + ".Shared", ProjectType.Shared);
                    {


                        var classB = new ClassFile("LauncherApp", "Rydia." + projectName , "AppRunner");
                        {
                            // usingを追加
                            classB.AddUsing("System");
                            classB.AddUsing("System.Collections.Generic");
                            classB.AddUsing("System.Linq");
                            classB.AddUsing("System.Text");
                            classB.AddUsing("System.Reflection");
                            classB.AddUsing("Rydia.Diagnostics");
                            classB.AddUsing("Rydia.Engine");
                            classB.AddUsing("Rydia.Runtime.Desktop");
                            classB.AddUsing("Rydia.Graphics.Api.ES30");
                            classB.AddUsing("Rydia.Runtime");
                            // クラスAにプロパティとメソッドを追加
                            classB.AddProperty("GL", "static IGLES30", """
                                get
                                {
                                    return RuntimeHost.GLES30;
                                }
                                """);

                            classB.AddOverrideMethod("Load", "// リソースを読み込む");
                            classB.AddOverrideMethod("Update", "// ゲームの状態を更新する", ("float", "deltaTimeSec"), ("float", "totalTimeSec"));
                            classB.AddOverrideMethod("Render", "// 描画処理");
                            classB.AddOverrideMethod("Unload", "// リソースを解放する");

                            projectC.AddClass(classB);
                        }


                        solutionFile.AddProject(projectC);
                    }

                    /*
                    // プロジェクトA: ライブラリを生成
                    var projectA = new ProjectFile(projectName, ProjectType.ClassLibrary);
                    {
                        // アセンブリ参照を追加
                        projectA.AddAssemblyReference("Rydia.Runtime", editordirectory + "/Rydia.Runtime.dll");
                        projectA.AddAssemblyReference("Rydia.Engine", editordirectory + "/Rydia.Engine.dll");
                        projectA.AddReference(projectC);
                        // クラスAを生成
                        var classA = new ClassFile("SampleClass");
                        {
                            // 名前空間を設定
                            classA.Namespace = "Rydia." + projectName;

                            // usingを追加
                            classA.AddUsing("System");
                            classA.AddUsing("System.Collections.Generic");
                            classA.AddUsing("System.Linq");
                            classA.AddUsing("System.Text");
                            classA.AddUsing("Rydia");

                            // クラスAにプロパティとメソッドを追加
                            classA.AddProperty("Name", "string");
                            classA.AddProperty("Age", "int");
                            classA.AddProperty("Enabled", "bool");
                            classA.AddProperty("Position", "Vector3");
                            classA.AddMethod("Initialize", """
    Name = "Player";
    Age = 32;
    """);
                            classA.AddMethod("GetName", "return Name;", "string");

                            // プロジェクトAにクラスAを追加
                            projectA.AddClass(classA);
                        }

                        solutionFile.AddProject(projectA);
                    }
                    */

                    // プロジェクトB: WindowsDebuggerを生成
                    var projectB = new ProjectFile(projectName + ".Windows.Debugger", ProjectType.WindowsApplication);
                    {
                        // アセンブリ参照を追加
                        projectB.AddAssemblyReference("Rydia.Runtime", editordirectory + "/Rydia.Runtime.dll");
                        projectB.AddAssemblyReference("Rydia.Engine", editordirectory + "/Rydia.Engine.dll");
                        projectB.AddAssemblyReference("Rydia.Runtime.Windows", editordirectory + "/Rydia.Runtime.Windows.dll");
                        projectB.AddReference(projectC);

                        // クラスAを生成
                        var classA = new ClassFile("Program");
                        {
                            // 名前空間を設定
                            classA.Namespace = "Rydia." + projectName + ".Windows.Debugger";

                            // usingを追加
                            classA.AddUsing("System");
                            classA.AddUsing("System.Collections.Generic");
                            classA.AddUsing("System.Linq");
                            classA.AddUsing("System.Text");
                            classA.AddUsing("System.Reflection");
                            classA.AddUsing("Rydia.Diagnostics");
                            classA.AddUsing("Rydia.Engine");
                            classA.AddUsing("Rydia.Runtime.Windows");
                            classA.AddUsing("Rydia.Runtime.Desktop");

                            // クラスAにプロパティとメソッドを追加
                            classA.AddMethod("Main", """
    // Dllディレクトリを設定する
    string path = Assembly.GetEntryAssembly().Location;
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.GetDirectoryName(path);
    path = Path.Combine(path, (IntPtr.Size == 8) ? "x64" : "x86");
    WindowsPlatform.SetDllDirectory(path);

    // このアプリケーションはWindowsプラットフォーム専用のアプリケーションであるため、
    // Windowsプラットフォームの初期化を行う
    ApplicationConfiguration.Initialize();
    var ConsoleLog = new ConsoleLogListener();
    GlobalLogger.AddListener(ConsoleLog);

    // DebuggerAppではログファイルを作成する
    {
        var fileLog = new FileLogListener("Logs\\Rydia.Runtime.Windows.App.log");
        GlobalLogger.AddListener(fileLog);
    }
    
    var app = new LauncherApp();
    using (var platform = new WindowsPlatform())
    {
        // ここではテストコードとしてウィンドウとオーディオの設定をリテラルコードで行っている

        // ウィンドウ設定
        // 最大幅768の[19:9]ウィンドウを作成
        var v = 768;
        var w = 19;
        var h = 9;
        var a = v / w;
        var b = a * h;
        platform.WindowOptions.Width = b;
        platform.WindowOptions.Height = 768;
        platform.WindowOptions.Title = "Rydia Runtime Windows App - " + app.GetType().Name;
        platform.WindowOptions.RefreshMode = RefreshMode.VSync;
        platform.WindowOptions.ScreenMode = ScreenMode.FixedWindow;

        // オーディオ設定
        // 既定のデバイスを使用
        platform.AudioOptions.DeviceName = null;
        // サンプリング周波数を44.1kHzに設定
        platform.AudioOptions.Frequency = 44100;
        // モノラル音源の数を16に設定
        platform.AudioOptions.MonoSources = 16;
        // ステレオ音源の数を16に設定
        platform.AudioOptions.StereoSources = 16;
        // 更新レートを60に設定
        platform.AudioOptions.Refresh = 60;
        // クロック同期を無効に設定
        platform.AudioOptions.Sync = false;
        platform.Run(app);
    }
    """, "static void");
                            // プロジェクトAにクラスAを追加
                            projectB.AddClass(classA);
                        }

                        solutionFile.AddProject(projectB);
                    }

                    // プロジェクトD: DroidDebuggerを生成
                    var projectD = new ProjectFile(projectName + ".Droid.Debugger", ProjectType.AndroidApplication);
                    {
                        // アセンブリ参照を追加
                        projectD.AddAssemblyReference("Rydia.Runtime", editordirectory + "/Rydia.Runtime.dll");
                        projectD.AddAssemblyReference("Rydia.Engine", editordirectory + "/Rydia.Engine.dll");
                        projectD.AddReference("Rydia.Runtime.Droid", "../../../../../../../../../../../Platform/Rydia.Runtime.Droid/Rydia.Runtime.Droid.csproj");
                        projectD.AddReference(projectC);

                        // クラスAを生成
                        var classA = new ClassFile("MainActivity", "Rydia." + projectName + ".Debugger", "GLActivity");
                        {
                            // usingを追加
                            classA.AddUsing("System");
                            classA.AddUsing("System.Collections.Generic");
                            classA.AddUsing("System.Linq");
                            classA.AddUsing("System.Text");
                            classA.AddUsing("System.Reflection");
                            classA.AddUsing("Rydia.Diagnostics");
                            classA.AddUsing("Rydia.Engine");
                            classA.AddUsing("Rydia.Runtime.Desktop");
                            classA.AddUsing("Rydia.Graphics.Api.ES30");
                            classA.AddUsing("Rydia.Runtime");
                            classA.AddUsing("Rydia.Runtime.Droid");

                            classA.AddClassAttribute("[Activity(Label = \"@string/app_name\", MainLauncher = true)]");
                            classA.AddConstructer("""
                                                        public MainActivity()
                                    : base(new LauncherApp())
                                {
                                    // 既定のデバイスを使用
                                    AudioOptions.DeviceName = null;
                                    // サンプリング周波数を44.1kHzに設定
                                    AudioOptions.Frequency = 44100;
                                    // モノラル音源の数を16に設定
                                    AudioOptions.MonoSources = 16;
                                    // ステレオ音源の数を16に設定
                                    AudioOptions.StereoSources = 16;
                                    // 更新レートを60に設定
                                    AudioOptions.Refresh = 60;
                                    // クロック同期を無効に設定
                                    AudioOptions.Sync = false;
                                }
                                """);

                            projectD.AddClass(classA);
                        }


                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\layout");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-anydpi-v26");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-hdpi");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-mdpi");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xhdpi");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxhdpi");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxxhdpi");
                        Directory.CreateDirectory($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\values");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\AndroidManifest.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<manifest xmlns:android=\"http://schemas.android.com/apk/res/android\">\r\n  <application android:allowBackup=\"true\" android:icon=\"@mipmap/appicon\" android:label=\"@string/app_name\" android:roundIcon=\"@mipmap/appicon_round\" android:supportsRtl=\"true\">\r\n  </application>\r\n  <uses-permission android:name=\"android.permission.INTERNET\" />\r\n</manifest>");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\layout\\activity_main.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<RelativeLayout xmlns:android=\"http://schemas.android.com/apk/res/android\"\r\n    xmlns:app=\"http://schemas.android.com/apk/res-auto\"\r\n    xmlns:tools=\"http://schemas.android.com/tools\"\r\n    android:layout_width=\"match_parent\"\r\n    android:layout_height=\"match_parent\">\r\n    <TextView \r\n        android:layout_width=\"wrap_content\"\r\n        android:layout_height=\"wrap_content\"\r\n        android:layout_centerInParent=\"true\"\r\n        android:text=\"@string/app_text\"\r\n    />\r\n</RelativeLayout>");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-anydpi-v26\\appicon.xml", "<adaptive-icon xmlns:android=\"http://schemas.android.com/apk/res/android\">\r\n  <background android:drawable=\"@mipmap/appicon_background\" />\r\n  <foreground android:drawable=\"@mipmap/appicon_foreground\" />\r\n</adaptive-icon>");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-anydpi-v26\\appicon_round.xml", "<adaptive-icon xmlns:android=\"http://schemas.android.com/apk/res/android\">\r\n  <background android:drawable=\"@mipmap/appicon_background\" />\r\n  <foreground android:drawable=\"@mipmap/appicon_foreground\" />\r\n</adaptive-icon>");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\values\\ic_launcher_background.xml", "<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n<resources>\r\n    <color name=\"ic_launcher_background\">#2C3E50</color>\r\n</resources>");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\values\\strings.xml", "<resources>\r\n    <string name=\"app_name\">Rydia.Runtime.Droid.App</string>\r\n    <string name=\"app_text\">Hello, Android!</string>\r\n</resources>\r\n");
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\AboutResources.txt", "Images, layout descriptions, binary blobs and string dictionaries can be included \r\nin your application as resource files.  Various Android APIs are designed to \r\noperate on the resource IDs instead of dealing with images, strings or binary blobs \r\ndirectly.\r\n\r\nFor example, a sample Android app that contains a user interface layout (main.xml),\r\nan internationalization string table (strings.xml) and some icons (drawable-XXX/icon.png) \r\nwould keep its resources in the \"Resources\" directory of the application:\r\n\r\nResources/\r\n    drawable/\r\n        icon.png\r\n\r\n    layout/\r\n        main.xml\r\n\r\n    values/\r\n        strings.xml\r\n\r\nIn order to get the build system to recognize Android resources, set the build action to\r\n\"AndroidResource\".  The native Android APIs do not operate directly with filenames, but \r\ninstead operate on resource IDs.  When you compile an Android application that uses resources, \r\nthe build system will package the resources for distribution and generate a class called \"Resource\" \r\n(this is an Android convention) that contains the tokens for each one of the resources \r\nincluded. For example, for the above Resources layout, this is what the Resource class would expose:\r\n\r\npublic class Resource {\r\n    public class Drawable {\r\n        public const int icon = 0x123;\r\n    }\r\n\r\n    public class Layout {\r\n        public const int main = 0x456;\r\n    }\r\n\r\n    public class Strings {\r\n        public const int first_string = 0xabc;\r\n        public const int second_string = 0xbcd;\r\n    }\r\n}\r\n\r\nYou would then use Resource.Drawable.icon to reference the drawable/icon.png file, or \r\nResource.Layout.main to reference the layout/main.xml file, or Resource.Strings.first_string \r\nto reference the first string in the dictionary file values/strings.xml.");

                        var embedded = new EmbeddedResources<EditorApp>();
                        var buffer1 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_hdpi.appicon.png");
                        var buffer2 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_hdpi.appicon_background.png");
                        var buffer3 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_hdpi.appicon_foreground.png");

                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-hdpi\\appicon.png", buffer1);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-hdpi\\appicon_background.png", buffer2);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-hdpi\\appicon_foreground.png", buffer3);

                        buffer1 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_mdpi.appicon.png");
                        buffer2 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_mdpi.appicon_background.png");
                        buffer3 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_mdpi.appicon_foreground.png");

                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-mdpi\\appicon.png", buffer1);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-mdpi\\appicon_background.png", buffer2);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-mdpi\\appicon_foreground.png", buffer3);

                        buffer1 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xhdpi.appicon.png");
                        buffer2 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xhdpi.appicon_background.png");
                        buffer3 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xhdpi.appicon_foreground.png");

                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xhdpi\\appicon.png", buffer1);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xhdpi\\appicon_background.png", buffer2);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xhdpi\\appicon_foreground.png", buffer3);

                        buffer1 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxhdpi.appicon.png");
                        buffer2 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxhdpi.appicon_background.png");
                        buffer3 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxhdpi.appicon_foreground.png");

                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxhdpi\\appicon.png", buffer1);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxhdpi\\appicon_background.png", buffer2);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxhdpi\\appicon_foreground.png", buffer3);

                        buffer1 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxxhdpi.appicon.png");
                        buffer2 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxxhdpi.appicon_background.png");
                        buffer3 = embedded.Load("Rydia.GameStudio.EmbeddedResources.mipmap_xxxhdpi.appicon_foreground.png");

                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxxhdpi\\appicon.png", buffer1);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxxhdpi\\appicon_background.png", buffer2);
                        projectD.AddFile($"Projects\\{projectName}\\Source\\Code\\{projectName}.Droid.Debugger\\Resources\\mipmap-xxxhdpi\\appicon_foreground.png", buffer3);

                        solutionFile.AddProject(projectD);
                    }

                    solutionFile.Generate();

                    logForm.AppendLog("ソリューション生成が完了しました。");
                    logForm.SetStatus("プロジェクトをビルドしています...");
                    logForm.AppendLog("ビルドを開始します。");

                    solutionFile.LogReceived += (message) =>
                    {
                        logForm.AppendLog(message);
                    };

                    solutionFile.Build();

                    logForm.AppendLog("ビルドが完了しました。");

                    logForm.AppendLog("生成処理が正常に終了しました。");
                    logForm.SetStatus("生成処理が完了しました。");

                    logForm.AppendLog("プロジェクトの読み込みを開始します。");
                    logForm.SetStatus("プロジェクトの読み込んでいます。");

                    logForm.AppendLog("プロジェクトを開始します。");
                    logForm.SetStatus("プロジェクトを開始しています。");
                    success = true;
                    Task.Delay(5000).Wait(); // 5秒待機してログを閉じる
                });

            }
            catch (Exception ex)
            {
                logForm.AppendLog($"エラー: {ex.Message}");
                logForm.AppendLog(ex.ToString());
                logForm.SetStatus("生成処理に失敗しました。");

                MessageBox.Show(
                    logForm,
                    $"プロジェクトの生成に失敗しました。\n\n{ex.Message}",
                    "生成エラー",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            finally
            {
                logForm.Complete(success);
                this.createButton.Enabled = true;
            }

            if (success)
            {

                await Task.Delay(5000);

                EditorContext.OpenProject(projectDirectory);
                // ソリューションを開く
                solutionFile.Open();

                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }
}
