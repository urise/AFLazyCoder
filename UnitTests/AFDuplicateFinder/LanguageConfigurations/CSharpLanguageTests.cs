using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFDuplicateFinder.LanguageConfigurations;
using NUnit.Framework;

namespace UnitTests.AFDuplicateFinder.LanguageConfigurations
{
    [TestFixture]
    class CSharpLanguageTests
    {
        #region PreliminaryProcess Tests

        [Test]
        public void PreliminaryProcess_Trim()
        {
            var input = new List<string> { "    \tabcde\t\t", " abc      " };
            var expected = new List<string> { "abcde", "abc" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_Tabs()
        {
            var input = new List<string> { "abc\td\te" };
            var expected = new List<string> { "abc d e" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_DoubleSpaces()
        {
            var input = new List<string> { "abc\t\td      e", "a   b   c\"   \"" };
            var expected = new List<string> { "abc d e", "a b c\"   \"" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_FirstTypeCommentsTest()
        {
            var input = new List<string> {"abcde", "abc//line comment", "\"//\"//line comment"};
            var expected = new List<string> {"abcde", "abc", "\"//\""};
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_SecondTypeCommentsSimpleTest()
        {
            var input = new List<string> { "abc/*comment*/def", "/*comm//ent*/abc", "abc/*comm1*/ def/*comm2*/",
                "abc/*comm1*////*comm2*/def"};
            var expected = new List<string> { "abcdef", "abc", "abc def", "abc" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_SecondTypeCommentsMultilineTest()
        {
            var input = new List<string> { "abc/*comment", "some nice text", "some another text", "bla-bla*/def/*comm*/gh" };
            var expected = new List<string> { "abc", "", "", "defgh" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_SecondTypeCommentsMultilineMixedTest()
        {
            var input = new List<string> { "abc///*comment", "some nice text", "some another text", "bla-bla//*/def" };
            var expected = new List<string> { "abc", "some nice text", "some another text", "bla-bla" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_AssembleOperators_2Lines()
        {
            var input = new List<string> { "abcde", "fghij;" };
            var expected = new List<string> {"abcde fghij;", ""};
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_AssembleOperators_ManyLines()
        {
            var input = new List<string> { "aaa;", "abcde", "fghij", "klmno", "pqrst;" };
            var expected = new List<string> { "aaa;", "abcde fghij klmno pqrst;", "", "", "" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_AssembleOperators_ManyLinesBrackets()
        {
            var input = new List<string> { "aaa;", "abcde", "fghij", "klmno", "pqrst", "{" };
            var expected = new List<string> { "aaa;", "abcde fghij klmno pqrst", "", "", "", "{" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_AssembleOperators_TwoAssembles()
        {
            var input = new List<string> { "aaa", "bbb;", "ccc", "ddd;" };
            var expected = new List<string> { "aaa bbb;", "", "ccc ddd;", "" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_Using()
        {
            var input = new List<string> {"using aab;", "usingAbb"};
            var expected = new List<string> {"", "usingAbb"};
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void PreliminaryProcess_Region()
        {
            var input = new List<string> { "#region abcde", "#regionion" };
            var expected = new List<string> { "", "#regionion" };
            var result = new CSharpLanguage().PreliminaryProcess(input);
            CollectionAssert.AreEqual(expected, result);
        }

        #endregion
    }
}
