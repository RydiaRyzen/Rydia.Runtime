using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{
    public class ProjectReferenceDefinition
    {
        public ProjectReferenceDefinition(string asmName, string path)
        {
            AsmName = asmName;
            Path = path;
        }

        public string AsmName { get; }
        public string Path { get; }
    }
}
