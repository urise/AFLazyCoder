using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFDuplicateFinder;
using NUnit.Framework;
using UnitTests.AFDuplicateFinder.Mocks;

namespace UnitTests.AFDuplicateFinder
{
    [TestFixture]
    public class DuplicateFinderTests
    {
        [Test]
        public void BasicNegativeTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee"})
                                   };
            var filesCombiner = new DuplicateFinder(fileContents);
            //when
            var result = filesCombiner.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(0));
        }

        [Test]
        public void BasicPositiveTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb", "ccc", "ddd", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "bbb", "ccc", "ddd", "zzzz"})
                                   };
            //when
            var filesCombiner = new DuplicateFinder(fileContents);
            var result = filesCombiner.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LinesCount, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(3));
        }

        [Test]
        public void BasicPositiveAnotherOrderTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "ddd", "ccc", "bbb", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "ddd", "ccc", "bbb", "zzzz"})
                                   };
            //when
            var filesCombiner = new DuplicateFinder(fileContents);
            var result = filesCombiner.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LinesCount, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(3));
        }

        [Test]
        public void ComplicatedPositiveTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb", "ccc", "ddd", "eee", "bbb", "ccc"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "bbb", "ccc", "ddd", "zzzz"}),
                                       new FileContent("3.txt", new List<string> {"kkk", "lll", "bbb", "ccc", "mmm"})
                                   };
            //when
            var filesCombiner = new DuplicateFinder(fileContents);
            var result = filesCombiner.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result[1].LinesCount, Is.EqualTo(3));
            Assert.That(result[1].GetUnitsCount(), Is.EqualTo(2));
            Assert.That(result[1].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[1].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[1].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[1].GetUnit(1).LineNumber, Is.EqualTo(3));

            Assert.That(result[0].LinesCount, Is.EqualTo(2));
            Assert.That(result[0].GetUnitsCount(), Is.EqualTo(4));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(6));
            Assert.That(result[0].GetUnit(2).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(2).LineNumber, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(3).FileName, Is.EqualTo("3.txt"));
            Assert.That(result[0].GetUnit(3).LineNumber, Is.EqualTo(3));
        }

        [Test]
        public void LowerLimitTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb", "ccc", "ddd", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "bbb", "ccc", "ddd", "zzzz"})
                                   };
            var parameters = new DuplicateFindParameters {NumberOfLinesLowerLimit = 4};
            //when
            var filesCombiner = new DuplicateFinder(fileContents, parameters);
            var result = filesCombiner.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(0));
        }

        #region Language Tests

        private DuplicateFinder GetMockLanguageDuplicateFinder(IEnumerable<FileContent> fileContents)
        {
            var language = new MockLanguage();
            var parameters = new DuplicateFindParameters(3, language);
            return new DuplicateFinder(fileContents, parameters);
        }

        [Test]
        public void CannotStartTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "??123", "bbb", "ccc", "ddd", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "??123", "bbb", "ccc", "ddd", "zzzz"})
                                   };
            var duplicateFinder = GetMockLanguageDuplicateFinder(fileContents);
            //when
            var result = duplicateFinder.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LinesCount, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(4));
        }

        [Test]
        public void CannotEndTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb", "ccc", "ddd", "!!!!", "", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "bbb", "ccc", "ddd", "!!!!", "", "zzzz"})
                                   };
            var duplicateFinder = GetMockLanguageDuplicateFinder(fileContents);
            //when
            var result = duplicateFinder.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LinesCount, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(3));
        }

        [Test]
        public void FullIgnoreTest()
        {
            //given
            var fileContents = new List<FileContent>
                                   {
                                       new FileContent("1.txt", new List<string> {"aaa", "bbb", "", "ccc", "// a", "", "", "ddd", "eee"}),
                                       new FileContent("2.txt", new List<string> {"aaaa", "eeee", "bbb", "", "", "// ", "ccc", "ddd", "zzzz"})
                                   };
            var duplicateFinder = GetMockLanguageDuplicateFinder(fileContents);
            //when
            var result = duplicateFinder.FindDuplicates();
            //then
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].LinesCount, Is.EqualTo(3));
            Assert.That(result[0].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(result[0].GetUnit(0).LineNumber, Is.EqualTo(2));
            Assert.That(result[0].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(result[0].GetUnit(1).LineNumber, Is.EqualTo(3));
        }

        #endregion
    }
}
