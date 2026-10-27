using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{

    public class TextFileDefinition
    {
        public TextFileDefinition(string fileName, string text)
        {
            FileName = fileName;
            Text = text;
        }

        public string FileName { get; }
        public string Text { get; }
    }
}
