using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder
{
    public class DuplicateInfo: IEnumerable<IndexPair>
    {
        private readonly List<IndexPair> _pairs = new List<IndexPair>();

        public void Add(int index1, int index2)
        {
            var pair = new IndexPair(index1, index2);
            if (_pairs.Count == 0 || _pairs[0].Index1 < index1)
                _pairs.Add(pair);
            else
                _pairs.Insert(0, pair);
        }

        public IndexPair GetFirstIndexPair()
        {
            return _pairs.FirstOrDefault();
        }

        public int GetLinesCount()
        {
            return _pairs.Count;
        }

        public void RemoveFirst()
        {
            if (_pairs.Any()) _pairs.RemoveAt(0);
        }

        public void RemoveLast()
        {
            if (_pairs.Any()) _pairs.RemoveAt(_pairs.Count - 1);
        }

        public IEnumerator<IndexPair> GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _pairs.GetEnumerator();
        }
    }
}
