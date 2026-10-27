using System;
using System.Collections.Generic;
using System.Text;

namespace Rydia.SolutionBuilder
{
    public class BinaryFileDefinition
    {
        public BinaryFileDefinition(string fileName, byte[] bytes)
        {
            FileName = fileName;
            Bytes = bytes;
        }

        public string FileName { get; }
        public byte[] Bytes { get; }
    }
}
