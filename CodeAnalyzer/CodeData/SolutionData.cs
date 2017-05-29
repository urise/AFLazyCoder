using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CodeAnalyzer.CodeData
{
    public class SolutionData
    {
        public string SolutionName { get; internal set; }
        public string SolutionFileName { get; internal set; }
        public string SolutionFolderName { get; internal set; }

        public List<ProjectData> Projects { get; internal set; }
    }
}
