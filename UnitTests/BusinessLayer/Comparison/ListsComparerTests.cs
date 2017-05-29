using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BusinessLayer.Comparison;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.Comparison
{
    [TestFixture]
    public class ListsComparerTests
    {
        [Test]
        public void CompareSortedTest()
        {
            //given
            var list1 = new List<int> {1, 3, 10, 20};
            var list2 = new List<int> {2, 3, 15, 20, 25};
            //when
            var listsComparer = new ListsComparer<int>(list1, list2);
            listsComparer.CompareSorted();
            //then
            CollectionAssert.AreEqual(new List<int> {1, 10}, listsComparer.OnlyInList1);
            CollectionAssert.AreEqual(new List<int> { 2, 15, 25 }, listsComparer.OnlyInList2);
        }

        [Test]
        public void CompareSortedOneEmptyTest()
        {
            //given
            var list1 = new List<int> ();
            var list2 = new List<int> { 2, 3, 15, 20, 25 };
            //when
            var listsComparer = new ListsComparer<int>(list1, list2);
            listsComparer.CompareSorted();
            //then
            CollectionAssert.IsEmpty(listsComparer.OnlyInList1);
            CollectionAssert.AreEqual(new List<int> { 2, 3, 15, 20, 25 }, listsComparer.OnlyInList2);
        }
    }
}
