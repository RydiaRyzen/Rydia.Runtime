using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Rydia.Diagnostics;
using Rydia.GameStudio.App;

namespace Rydia.GameStudio
{

    public sealed class ConsoleView : EditorDockContent
    {

        private readonly ListView listView;
        private readonly ComboBox filterComboBox;
        private readonly List<LogEntry> entries = new List<LogEntry>();

        private static readonly string[] Categories =
    {
        "All",
        "CoreLog",
        "EditorLog",
        "GameLog",
        "Error",
        "Warning",
        "Debug",
        "Info"
    };

        public ListView ListView
        {
            get { return this.listView; }
        }

        public ConsoleView()
            : base("Console")
        {
            this.listView = new ListView
            {
                Dock = DockStyle.Fill,
                View = View.Details,
                FullRowSelect = true,
                GridLines = true,
                HideSelection = false,
                MultiSelect = true,
                BorderStyle = BorderStyle.None,
                BackColor = Color.FromArgb(30, 32, 39),
                ForeColor = Color.FromArgb(220, 224, 232)
            };

            this.listView.Columns.Add("Time", 100);
            this.listView.Columns.Add("Type", 100);
            this.listView.Columns.Add("Message", 700);

            var toolbar = new ToolStrip
            {
                Dock = DockStyle.Top,
                GripStyle = ToolStripGripStyle.Hidden,
                BackColor = Color.FromArgb(38, 41, 49),
                ForeColor = Color.White,
                Padding = new Padding(4)
            };

            this.filterComboBox = new ComboBox
            {
                DropDownStyle = ComboBoxStyle.DropDownList,
                Width = 130
            };
            this.filterComboBox.Items.AddRange(Categories);
            this.filterComboBox.SelectedIndex = 0;
            this.filterComboBox.SelectedIndexChanged += (_, _) => RefreshLogs();

            var filterHost = new ToolStripControlHost(this.filterComboBox)
            {
                AutoSize = false,
                Width = 135,
                Height = 25
            };

            toolbar.Items.Add(new ToolStripLabel("Filter"));
            toolbar.Items.Add(filterHost);
            toolbar.Items.Add(new ToolStripSeparator());

            var clearButton = new ToolStripButton("Clear");
            clearButton.Click += (_, _) => Clear();
            toolbar.Items.Add(clearButton);

            Controls.Add(this.listView);
            Controls.Add(toolbar);

            foreach (var listener in GlobalLogger.Listeners)
            {
                if (listener is EditorLogListener textBoxListener)
                {
                    foreach (var log in textBoxListener.Messages)
                    {
                        Log(log);
                    }
                    textBoxListener.ConsoleView = this;
                }
            }
        }

        internal void Log(ILogMessage message)
        {
            if(message.Source.Module == "Editor")
            {
                Log(message, "EditorLog");
            }
            else if (message.Source.Module == "Core")
            {
                Log(message, "CoreLog");
            }
            else if (message.Source.Module == "Game")
            {
                Log(message, "GameLog");
            }
            else
            {
                Log(message, null);
            }
        }

        /// <summary>
        /// ログを指定された発生元で追加します。
        /// </summary>
        internal void Log(ILogMessage message, string source)
        {
            if (message == null)
                return;

            string text = message.Message;
            string category = Classify(message, text, source);

            var entry = new LogEntry
            {
                Time = message.TimeStamp,
                Category = category,
                Message = text
            };

            if (this.listView.InvokeRequired)
            {
                this.listView.BeginInvoke(new Action(() => AddEntry(entry)));
            }
            else
            {
                AddEntry(entry);
            }
        }

        private void AddEntry(LogEntry entry)
        {
            this.entries.Add(entry);

            if (IsVisible(entry))
                AddListItem(entry);

            // 表示件数が増えすぎないようにする
            const int maxEntries = 10000;

            if (this.entries.Count > maxEntries)
            {
                this.entries.RemoveAt(0);
                RefreshLogs();
            }
        }

        private void AddListItem(LogEntry entry)
        {
            var item = new ListViewItem(
                entry.Time.ToString("HH:mm:ss.fff"));

            item.SubItems.Add(entry.Category);
            item.SubItems.Add(entry.Message);

            item.Group = GetGroup(entry.Category);

            switch (entry.Category)
            {
                case "Error":
                    item.ForeColor = Color.FromArgb(255, 100, 100);
                    break;

                case "Warning":
                    item.ForeColor = Color.FromArgb(255, 195, 90);
                    break;

                case "Debug":
                    item.ForeColor = Color.FromArgb(150, 175, 200);
                    break;
            }

            this.listView.Items.Add(item);
        }

        private ListViewGroup GetGroup(string category)
        {
            foreach (ListViewGroup group in this.listView.Groups)
            {
                if (group.Name == category)
                    return group;
            }

            var newGroup = new ListViewGroup(category, category);
            this.listView.Groups.Add(newGroup);

            return newGroup;
        }

        private bool IsVisible(LogEntry entry)
        {
            string selected = this.filterComboBox.SelectedItem as string;

            return string.IsNullOrEmpty(selected)
                || selected == "All"
                || entry.Category == selected;
        }

        private void RefreshLogs()
        {
            this.listView.BeginUpdate();

            try
            {
                this.listView.Items.Clear();
                this.listView.Groups.Clear();

                foreach (var entry in this.entries)
                {
                    if (IsVisible(entry))
                        AddListItem(entry);
                }
            }
            finally
            {
                this.listView.EndUpdate();
            }
        }

        /// <summary>
        /// ログをすべて消去します。
        /// </summary>
        public void Clear()
        {
            this.entries.Clear();
            this.listView.Items.Clear();
            this.listView.Groups.Clear();
        }

        private static string Classify(
            ILogMessage message,
            string text,
            string source)
        {
            string typeName = message.GetType().Name;

            string level = message.Level.ToString();

            string metadata = (typeName + " " + level + " " + text)
                .ToLowerInvariant();

            // 重要度をログの発生元より優先
            if (ContainsAny(metadata,
                "fatal", "critical", "exception", "error"))
                return "Error";

            if (ContainsAny(metadata,
                "warning", "warn"))
                return "Warning";

            if (ContainsAny(metadata,
                "debug", "trace"))
                return "Debug";

            if (!string.IsNullOrEmpty(source))
            {
                if (source.Equals("Core",
                    StringComparison.OrdinalIgnoreCase))
                    return "Core";

                if (source.Equals("Editor",
                    StringComparison.OrdinalIgnoreCase))
                    return "Editor";

                if (source.Equals("Game",
                    StringComparison.OrdinalIgnoreCase))
                    return "Game";
            }

            if (typeName.IndexOf("Core",
                StringComparison.OrdinalIgnoreCase) >= 0)
                return "Core";

            if (typeName.IndexOf("Editor",
                StringComparison.OrdinalIgnoreCase) >= 0)
                return "Editor";

            if (typeName.IndexOf("Game",
                StringComparison.OrdinalIgnoreCase) >= 0)
                return "Game";

            return "Info";
        }

        private static bool ContainsAny(string text, params string[] values)
        {
            return values.Any(value => text.Contains(value));
        }

        private static string GetPropertyString(
            object instance, string propertyName)
        {
            PropertyInfo property = instance.GetType().GetProperty(
                propertyName,
                BindingFlags.Public | BindingFlags.Instance);

            return property?.GetValue(instance)?.ToString();
        }

        private sealed class LogEntry
        {
            public DateTime Time { get; set; }
            public string Category { get; set; }
            public string Message { get; set; }
        }
    }

}
