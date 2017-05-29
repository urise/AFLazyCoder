using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder
{
    public class FileLine
    {
        public string FileName { get; private set; }
        public string Line { get; private set; }
        public int LineNumber { get; private set; }
        public int IndexInUnsortedArray { get; private set; }

        private readonly List<int> _partialLinks = new List<int>();
        private readonly List<int> _includedInResults = new List<int>(); 

        public void AddPartialLink(int indexInUnsortedArray)
        {
            _partialLinks.Add(indexInUnsortedArray);
        }

        public bool IsPartiallyLinked(int indexInUnsortedArray)
        {
            return _partialLinks.Contains(indexInUnsortedArray);
        }

        public void AddResult(int resultIndex)
        {
            if (!_includedInResults.Contains(resultIndex))
                _includedInResults.Add(resultIndex);
        }

        public int GetResultsCount()
        {
            return _includedInResults.Count;
        }

        public int GetResultIndex(int n)
        {
            return _includedInResults[n];
        }

        public bool IsIncludedInResult(int resultIndex)
        {
            return _includedInResults.Contains(resultIndex);
        }

        public FileLine(string fileName, string line, int lineNumber, int indexInUnsortedArray)
        {
            FileName = fileName;
            Line = line;
            LineNumber = lineNumber;
            IndexInUnsortedArray = indexInUnsortedArray;
        }
    }
}
