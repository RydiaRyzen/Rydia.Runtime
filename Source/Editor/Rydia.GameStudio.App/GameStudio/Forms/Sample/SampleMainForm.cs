using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Rydia.Diagnostics;
using WeifenLuo.WinFormsUI.Docking;

namespace Rydia.GameStudio
{
    public partial class SampleMainForm : Form
    {

        private readonly DockPanel dockPanel;

        private readonly SceneView sceneView;
        private readonly HierarchyView hierarchyView;
        private readonly InspectorView inspectorView;
        private readonly ProjectView projectView;
        private readonly ConsoleView consoleView;

        public SampleMainForm()
        {
            InitializeComponent();
            Text = "Rydia GameStudio - " + EditorContext.ProjectName;
            StartPosition = FormStartPosition.CenterScreen;
            ClientSize = new Size(1440, 900);
            MinimumSize = new Size(1000, 650);

            BackColor = Color.FromArgb(30, 32, 39);
            ForeColor = Color.White;

            var menu = CreateMenu();
            var toolbar = CreateToolbar();

            this.dockPanel = new DockPanel
            {
                Dock = DockStyle.Fill,
                DocumentStyle = DocumentStyle.DockingMdi,
                BackColor = Color.FromArgb(30, 32, 39),
                //Theme = new VS2015DarkTheme()
            };

            Controls.Add(this.dockPanel);
            Controls.Add(toolbar);
            Controls.Add(menu);

            MainMenuStrip = menu;

            toolbar.Dock = DockStyle.Top;
            menu.Dock = DockStyle.Top;

            this.sceneView = new SceneView();
            this.hierarchyView = new HierarchyView();
            this.inspectorView = new InspectorView();
            this.projectView = new ProjectView();
            this.consoleView = new ConsoleView();

            // ドッキング配置
            // 左上：シーン内のオブジェクト階層
            this.hierarchyView.Show(
                this.dockPanel,
                DockState.DockLeft);

            // 左下：プロジェクトのアセット
            this.projectView.Show(
                this.hierarchyView.Pane,
                DockAlignment.Bottom,
                0.45);

            // 中央：シーン描画領域
            this.sceneView.Show(
                this.dockPanel,
                DockState.Document);

            // 右側：オブジェクトのプロパティ
            this.inspectorView.Show(
                this.dockPanel,
                DockState.DockRight);

            // 下部：ログ
            this.consoleView.Show(
                this.dockPanel,
                DockState.DockBottom);

            // 選択オブジェクトを Inspector に反映
            this.hierarchyView.ObjectSelected += obj =>
            {
                this.inspectorView.SetObject(obj);
            };
        }

        public void MainLoop()
        {
            if (this.sceneView != null)
            {
                this.sceneView.MainLoop();
            }
        }

        private MenuStrip CreateMenu()
        {
            var menu = new MenuStrip
            {
                BackColor = Color.FromArgb(30, 32, 39),
                ForeColor = Color.White
            };

            foreach (string name in new[]
            {
                "ファイル", "編集", "表示",
                "実行", "設定", "ヘルプ"
            })
            {
                menu.Items.Add(name);
            }

            return menu;
        }

        private ToolStrip CreateToolbar()
        {
            var toolbar = new ToolStrip
            {
                BackColor = Color.FromArgb(38, 41, 49),
                ForeColor = Color.White,
                GripStyle = ToolStripGripStyle.Hidden,
                Padding = new Padding(5)
            };

            toolbar.Items.Add("Save");
            toolbar.Items.Add(new ToolStripSeparator());

            var playButton = new ToolStripButton("実行");
            var pauseButton = new ToolStripButton("一時停止");
            var stopButton = new ToolStripButton("停止");

            playButton.Click += (_, _) =>
               GlobalLogger.Editor.Debug("Play requested.");

            pauseButton.Click += (_, _) =>
               GlobalLogger.Editor.Debug(
                    "Pause requested.");

            stopButton.Click += (_, _) =>
                GlobalLogger.Editor.Debug(
                    "Stop requested.");

            toolbar.Items.Add(playButton);
            toolbar.Items.Add(pauseButton);
            toolbar.Items.Add(stopButton);

            return toolbar;
        }

    }
}
