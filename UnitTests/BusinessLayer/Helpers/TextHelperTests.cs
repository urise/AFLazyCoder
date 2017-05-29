using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.Helpers
{
    [TestFixture]
    class TextHelperTests
    {
        [Test]
        public void ConvertToConstNameTest()
        {
            Assert.AreEqual("ONE_TWO_THREE", "OneTwoThree".ConvertToConstName());
            Assert.AreEqual("ONE_TWO_THREE", "One_TwoTHREE".ConvertToConstName());
        }

        #region ExtractStringLiterals Tests

        [Test]
        public void ExtractStringLiterals_NullArg()
        {
            var result = TextHelper.ExtractStringLiterals(null);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ExtractStringLiterals_EmptyArg()
        {
            var result = TextHelper.ExtractStringLiterals(string.Empty);
            Assert.IsEmpty(result);
        }

        [Test]
        public void ExtractStringLiterals_NoLiterals()
        {
            var result = TextHelper.ExtractStringLiterals("   I like green 'apples'");
            Assert.IsEmpty(result);
        }

        [Test]
        public void ExtractStringLiterals_SimpleLiteral()
        {
            var result = TextHelper.ExtractStringLiterals(
                "   I like green \"apples\" and (nothing) {more}");
            CollectionAssert.AreEqual(new List<string> { "apples" }, result);
        }

        [Test]
        public void ExtractStringLiterals_LiteralWithQuotas()
        {
            var result = TextHelper.ExtractStringLiterals(
                "   I like green \"apples \\\"golden\\\"\" and (nothing) {more}");
            CollectionAssert.AreEqual(new List<string> { "apples \\\"golden\\\"" }, result);
        }

        [Test]
        public void ExtractStringLiterals_SeveralLiterals()
        {
            var result = TextHelper.ExtractStringLiterals(
                "   I like green \"apples \\\"golden\\\"\", \"oranges\", \"something \\\"strange\\\"\" and (nothing) {more}");
            CollectionAssert.AreEqual(
                new List<string> { "apples \\\"golden\\\"", "oranges", "something \\\"strange\\\"" },
                result);
        }

        [Test]
        public void ExtractStringLiterals_ManyQuotasLiterals()
        {
            var result = TextHelper.ExtractStringLiterals("\"\\\"\\\"\\\"\"");
            CollectionAssert.AreEqual(
                new List<string> { "\\\"\\\"\\\"" },
                result);
        }

        [Test]
        public void ExtractStringLiterals_IncludeApostrophesSimple()
        {
            var result = TextHelper.ExtractStringLiterals("   I like green 'apples'", true);
            CollectionAssert.AreEqual(new List<string> { "apples" }, result);
        }

        [Test]
        public void ExtractStringLiterals_IncludeApostrophesMixed()
        {
            var result = TextHelper.ExtractStringLiterals("  'aaa' \"bbb\" 'ccc' \"ddd\"", true);
            CollectionAssert.AreEqual(new List<string> { "aaa", "bbb", "ccc", "ddd" }, result);
        }

        [Test]
        public void ExtractStringLiterals_IncludeApostrophesMixedHardly()
        {
            var result = TextHelper.ExtractStringLiterals("  'aaa\"' \"'bbb'\"", true);
            CollectionAssert.AreEqual(new List<string> { "aaa\"", "'bbb'" }, result);
        }

        #endregion

        #region ExtractByPattern Tests

        [Test]
        public void ExtractByPattern_Several()
        {
            var result = TextHelper.ExtractByPattern("argument guano guns", "gu[^mn]*[mn]");
            CollectionAssert.AreEqual(
                new List<string>{"gum", "guan", "gun"},
                result);
        }

        #endregion

        #region TransformStringLiterals Tests

        [Test]
        public void TransformStringLiteralsVacuousTest()
        {
            const string input = "  abc def (){}";
            var result = input.TransformStringLiterals();
            Assert.That(result, Is.EqualTo(input));
        }

        [Test]
        public void TransformStringLiteralsSimpleTest()
        {
            const string input = "abc \"abcd\" def";
            var result = input.TransformStringLiterals();
            Assert.That(result, Is.EqualTo("abc \"    \" def"));
        }

        [Test]
        public void TransformStringLiteralsSimpleCustomParameterTest()
        {
            const string input = "abc \"abcd\" def";
            var result = input.TransformStringLiterals('1');
            Assert.That(result, Is.EqualTo("abc \"1111\" def"));
        }

        [Test]
        public void TransformStringLiteralsSimpleQuotasInsideTest()
        {
            const string input = "abc \"ab\\\"cd\" def";
            var result = input.TransformStringLiterals('1');
            Assert.That(result, Is.EqualTo("abc \"111111\" def"));
        }

        [Test]
        public void TransformStringLiteralsSeveralTest()
        {
            const string input = "abc \"ab\\\"cd\" \"012\" def \"4567\" gh";
            var result = input.TransformStringLiterals('1');
            Assert.That(result, Is.EqualTo("abc \"111111\" \"111\" def \"1111\" gh"));
        }

        #endregion

        #region GatherStringLiteralsAndReplaceToEmpty and PutGatheredLiteralsBack Tests

        [Test]
        public void GatherStringLiteralsAndReplaceToEmptyTest()
        {
            //given
            var lines = new List<string>
                {
                    " abcd",
                    "aaa\"qwerty\",\"12345\\\"678\" the end",
                    "bcd\"098 \" 'qw'efg\"\"hij",
                    " '\\\'' \"'''\""
                };
            //when
            var result = TextHelper.GatherStringLiteralsAndReplaceToEmpty(lines);
            //then
            var expectedResult = new List<string> { "qwerty", "12345\\\"678", "098 ", "qw", "", "\\'", "'''" };
            var expectedLines = new List<string> {" abcd", "aaa\"\",\"\" the end", "bcd\"\" ''efg\"\"hij", " '' \"\""};
            CollectionAssert.AreEqual(expectedResult, result);
            CollectionAssert.AreEqual(expectedLines, lines);
        }

        [Test]
        public void PutGatheredLiteralsBackTest()
        {
            //given
            var lines = new List<string> { " abcd", "aaa\"\",\"\" the end", "bcd\"\" ''efg\"\"hij", " '' \"\"" };
            var literals = new List<string> { "qwerty", "12345\\\"678", "098 ", "qw", "", "\\'", "'''" };
            //when
            TextHelper.PutGatheredLiteralsBack(lines, literals);
            //then
            var expectedLines = new List<string>
                {
                    " abcd",
                    "aaa\"qwerty\",\"12345\\\"678\" the end",
                    "bcd\"098 \" 'qw'efg\"\"hij",
                    " '\\\'' \"'''\""
                };
            CollectionAssert.AreEqual(expectedLines, lines);
        }

        #endregion

    }
}
