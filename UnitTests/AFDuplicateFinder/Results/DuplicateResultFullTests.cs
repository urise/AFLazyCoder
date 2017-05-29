using AFDuplicateFinder;
using AFDuplicateFinder.Results;
using NUnit.Framework;

namespace UnitTests.AFDuplicateFinder.Results
{
    [TestFixture]
    public class DuplicateResultFullTests
    {
        private DuplicateResultFull GetGivenResult()
        {
            var duplicateResultFull = new DuplicateResultFull();
            var duplicateResult1 = new DuplicateResult(5);
            duplicateResult1.AddUnit("1.txt", 10);
            duplicateResult1.AddUnit("2.txt", 20);
            duplicateResult1.AddUnit("3.txt", 30);
            duplicateResultFull.Add(duplicateResult1);
            var duplicateResult2 = new DuplicateResult(20);
            duplicateResult2.AddUnit("1.txt", 100);
            duplicateResult2.AddUnit("1.txt", 200);
            duplicateResult2.AddUnit("2.txt", 50);
            duplicateResultFull.Add(duplicateResult2);
            return duplicateResultFull;
        }

        [Test]
        public void IsPartOfAnotherResultBasicTest()
        {
            //given
            var duplicateResultFull = GetGivenResult();
            //then
            Assert.IsTrue(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 12, "3.txt", 32)));
            Assert.IsFalse(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 12, "3.txt", 34)));
            Assert.IsFalse(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 17, "3.txt", 37)));
            Assert.IsTrue(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 115, "1.txt", 215)));
            Assert.IsTrue(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 110, "2.txt", 60)));
            Assert.IsFalse(duplicateResultFull.IsPartOfAnotherResult(
                new PairOfPlaces("1.txt", 120, "1.txt", 220)));
        }

        [Test]
        public void AddNewResultTest()
        {
            //given
            var duplicateResultFull = GetGivenResult();
            var fileLine1 = new FileLine("1.txt", "aaa", 50, 1);
            var fileLine2 = new FileLine("2.txt", "aaa", 60, 1);
            //when
            duplicateResultFull.Add(5, fileLine1, fileLine2);
            //then
            Assert.That(duplicateResultFull.Count, Is.EqualTo(3));
            Assert.That(duplicateResultFull[2].LinesCount, Is.EqualTo(5));
            Assert.That(duplicateResultFull[2].GetUnitsCount(), Is.EqualTo(2));
            Assert.That(duplicateResultFull[2].GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(duplicateResultFull[2].GetUnit(0).LineNumber, Is.EqualTo(50));
            Assert.That(duplicateResultFull[2].GetUnit(1).FileName, Is.EqualTo("2.txt"));
            Assert.That(duplicateResultFull[2].GetUnit(1).LineNumber, Is.EqualTo(60));

            Assert.IsTrue(fileLine1.IsIncludedInResult(2));
            Assert.IsTrue(fileLine2.IsIncludedInResult(2));
        }

        [Test]
        public void AddPartiallyNewResultTest()
        {
            //given
            var duplicateResultFull = GetGivenResult();
            var fileLine1 = new FileLine("1.txt", "aaa", 10, 1);
            fileLine1.AddResult(0);
            var fileLine2 = new FileLine("4.txt", "aaa", 60, 1);
            //when
            duplicateResultFull.Add(5, fileLine1, fileLine2);
            //then
            Assert.That(duplicateResultFull.Count, Is.EqualTo(2));
            Assert.That(duplicateResultFull[0].GetUnitsCount(), Is.EqualTo(4));
            Assert.That(duplicateResultFull[0].GetUnit(3).FileName, Is.EqualTo("4.txt"));
            Assert.That(duplicateResultFull[0].GetUnit(3).LineNumber, Is.EqualTo(60));
            Assert.That(duplicateResultFull[1].GetUnitsCount(), Is.EqualTo(3));

            Assert.IsTrue(fileLine2.IsIncludedInResult(0));
        }

        [Test]
        public void AddExistingResultTest()
        {
            //given
            var duplicateResultFull = GetGivenResult();
            var fileLine1 = new FileLine("1.txt", "aaa", 100, 1);
            fileLine1.AddResult(1);
            var fileLine2 = new FileLine("1.txt", "aaa", 200, 1);
            //when
            duplicateResultFull.Add(20, fileLine1, fileLine2);
            //then
            Assert.That(duplicateResultFull.Count, Is.EqualTo(2));
            Assert.That(duplicateResultFull[0].GetUnitsCount(), Is.EqualTo(3));
            Assert.That(duplicateResultFull[1].GetUnitsCount(), Is.EqualTo(3));
        }
    }
}
