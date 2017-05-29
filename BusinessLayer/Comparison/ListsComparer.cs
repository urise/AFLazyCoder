using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace BusinessLayer.Comparison
{
    public class ListsComparer<T> where T: IComparable
    {
        #region Fields, Properties and Constructors

        private List<T> _list1, _list2;
        private readonly List<T> _onlyInList1 = new List<T>();
        private readonly List<T> _onlyInList2 = new List<T>();

        public ReadOnlyCollection<T> OnlyInList1 { get { return _onlyInList1.AsReadOnly(); } }
        public ReadOnlyCollection<T> OnlyInList2 { get { return _onlyInList2.AsReadOnly(); } }

        public ListsComparer(IEnumerable<T> list1, IEnumerable<T> list2)
        {
            _list1 = list1.ToList();
            _list2 = list2.ToList();
        }

        #endregion

        #region Methods

        public void CompareSorted()
        {
            int index1 = 0, index2 = 0;
            while (index1 < _list1.Count || index2 < _list2.Count)
            {
                if (index1 == _list1.Count)
                {
                    _onlyInList2.AddRange(_list2.GetRange(index2, _list2.Count - index2));
                    break;
                }

                if (index2 == _list2.Count)
                {
                    _onlyInList1.AddRange(_list1.GetRange(index1, _list1.Count - index1));
                    break;
                }

                var compareResult = _list1[index1].CompareTo(_list2[index2]);
                if (compareResult < 0) _onlyInList1.Add(_list1[index1++]);
                else if (compareResult > 0) _onlyInList2.Add(_list2[index2++]);
                else
                {
                    index1++;
                    index2++;
                }
            }
        }

        public void CompareUnsorted()
        {
            _list1.Sort();
            _list2.Sort();
            CompareSorted();
        }

        #endregion
    }
}
