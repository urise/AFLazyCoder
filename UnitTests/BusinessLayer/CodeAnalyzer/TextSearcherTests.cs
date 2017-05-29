using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.CodeAnalyzer;
using BusinessLayer.CodeAnalyzer.Searching;
using BusinessLayer.Helpers;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.CodeAnalyzer
{
    [TestFixture]
    public class TextSearcherTests
    {
        private TextSearcher GetTestingObject()
        {
            var result = new TextSearcher(s => TextHelper.ExtractStringLiterals(s));
            result.AddInfo("a", "1.txt", 5);
            result.AddInfo("bb", "1.txt", 10);
            result.AddInfo("a", "1.txt", 15);
            result.AddInfo("ccc", "2.txt", 8);
            result.AddInfo("dddd", "2.txt", 1);
            result.AddInfo("ccc", "2.txt", 5);
            result.AddInfo("ccc", "1.txt", 25);
            result.AddInfo("a", "3.txt", 7);
            result.AddInfo("ccc", "3.txt", 14);
            result.AddInfo("dddd", "1.txt", 21);
            return result;
        }

        [Test]
        public void GetFormattedReport_BriefOrderedByQuantityLimit0()
        {
            var reportParameters = new SearchReportParameters(false, TextSearcherReportOrder.Quantity, 0);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"ccc\": 4",
                                     "\"a\": 3",
                                     "\"dddd\": 2",
                                     "\"bb\": 1"
                                 }, report);
        }

        [Test]
        public void GetFormattedReport_BriefOrderedByQuantityLimit3()
        {
            var reportParameters = new SearchReportParameters(false, TextSearcherReportOrder.Quantity, 3);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"ccc\": 4",
                                     "\"a\": 3"
                                 }, report);
        }

        [Test]
        public void GetFormattedReport_BriefOrderedByLengthLimit0()
        {
            var reportParameters = new SearchReportParameters(false, TextSearcherReportOrder.Length, 0);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"dddd\": 2",
                                     "\"ccc\": 4",
                                     "\"bb\": 1",
                                     "\"a\": 3"
                                 }, report);
        }

        [Test]
        public void GetFormattedReport_DetailedOrderedByQuantityLimit0()
        {
            var reportParameters = new SearchReportParameters(true, TextSearcherReportOrder.Quantity, 0);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"ccc\": 4",
                                     "\t(2) 2.txt : 5, 8",
                                     "\t(1) 1.txt : 25",
                                     "\t(1) 3.txt : 14",
                                     "\"a\": 3",
                                     "\t(2) 1.txt : 5, 15",
                                     "\t(1) 3.txt : 7",
                                     "\"dddd\": 2",
                                     "\t(1) 1.txt : 21",
                                     "\t(1) 2.txt : 1",
                                     "\"bb\": 1",
                                     "\t(1) 1.txt : 10",
                                 }, report);
        }

        [Test]
        public void GetFormattedReport_DetailedOrderedByQuantityLimit3()
        {
            var reportParameters = new SearchReportParameters(true, TextSearcherReportOrder.Quantity, 3);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"ccc\": 4",
                                     "\t(2) 2.txt : 5, 8",
                                     "\t(1) 1.txt : 25",
                                     "\t(1) 3.txt : 14",
                                     "\"a\": 3",
                                     "\t(2) 1.txt : 5, 15",
                                     "\t(1) 3.txt : 7"
                                 }, report);
        }

        [Test]
        public void GetFormattedReport_DetailedOrderedByLengthLimit0()
        {
            var reportParameters = new SearchReportParameters(true, TextSearcherReportOrder.Length, 0);
            var report = GetTestingObject().GetFormattedReport(reportParameters);
            CollectionAssert.AreEqual(new[]
                                 {
                                     "\"dddd\": 2",
                                     "\t(1) 1.txt : 21",
                                     "\t(1) 2.txt : 1",
                                     "\"ccc\": 4",
                                     "\t(2) 2.txt : 5, 8",
                                     "\t(1) 1.txt : 25",
                                     "\t(1) 3.txt : 14",
                                     "\"bb\": 1",
                                     "\t(1) 1.txt : 10",
                                     "\"a\": 3",
                                     "\t(2) 1.txt : 5, 15",
                                     "\t(1) 3.txt : 7",
                                 }, report);
        }
    }
}
