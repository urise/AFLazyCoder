using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder
{
    public class FileContent
    {
        public string FileName { get; private set; }
        public IEnumerable<string> Lines { get { return _lines; } }

        private readonly List<string> _lines;

        public FileContent(string fileName, IEnumerable<string> lines)
        {
            FileName = fileName;
            _lines = new List<string>();
            _lines.AddRange(lines);
        }
    }
}
