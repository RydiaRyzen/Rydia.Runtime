using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Rydia.Diagnostics;

namespace Rydia.GameStudio.App
{
    public class EditorLogListener : LogListener
    {

        public List<ILogMessage> Messages
        {
            get;
        } = new List<ILogMessage>();

        public ConsoleView ConsoleView
        {
            get;
            set;
        }

        public override void Log(ILogMessage message)
        {
            if (ConsoleView != null)
            {
                ConsoleView.Log(message);
            }
            else
            {
                Messages.Add(message);
            }
        }

        protected override void WriteLine(ILogMessage message, string formattedLine)
        {

        }

    }
}
