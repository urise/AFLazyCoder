using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder
{
    public class PairOfPlaces
    {
        public string FileName1 { get; private set; }
        public int LineNumber1 { get; private set; }
        public string FileName2 { get; private set; }
        public int LineNumber2 { get; private set; }

        public PairOfPlaces(string fileName1, int lineNumber1, string fileName2, int lineNumber2)
        {
            FileName1 = fileName1;
            LineNumber1 = lineNumber1;
            FileName2 = fileName2;
            LineNumber2 = lineNumber2;
        }
    }
}
