using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder
{
    public class IndexPair
    {
        public int Index1 { get; private set; }
        public int Index2 { get; private set; }

        public IndexPair(int index1, int index2)
        {
            Index1 = index1;
            Index2 = index2;
        }
    }
}
