using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.CodeAnalyzer.Searching
{
    public class PlaceReportInfo
    {
        public string FileName { get; set; }
        public string LineNumbers { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            return string.Format("\t({0}) {1} : {2}", Count, FileName, LineNumbers);
        }
    }
}
