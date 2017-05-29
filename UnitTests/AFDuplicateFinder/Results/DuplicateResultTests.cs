using AFDuplicateFinder.Results;
using NUnit.Framework;

namespace UnitTests.AFDuplicateFinder.Results
{
    [TestFixture]
    public class DuplicateResultTests
    {
        [Test]
        public void AddUnitTest()
        {
            //given
            var duplicateResult = new DuplicateResult(5);
            //when
            duplicateResult.AddUnit("2.txt", 100);
            //then
            Assert.That(duplicateResult.GetUnitsCount(), Is.EqualTo(1));
            Assert.That(duplicateResult.GetUnit(0).FileName, Is.EqualTo("2.txt"));
            Assert.That(duplicateResult.GetUnit(0).LineNumber, Is.EqualTo(100));
            
            //when
            duplicateResult.AddUnit("4.txt", 50);
            //then
            Assert.That(duplicateResult.GetUnitsCount(), Is.EqualTo(2));
            Assert.That(duplicateResult.GetUnit(1).FileName, Is.EqualTo("4.txt"));
            Assert.That(duplicateResult.GetUnit(1).LineNumber, Is.EqualTo(50));

            //when
            duplicateResult.AddUnit("1.txt", 40);
            //then
            Assert.That(duplicateResult.GetUnitsCount(), Is.EqualTo(3));
            Assert.That(duplicateResult.GetUnit(0).FileName, Is.EqualTo("1.txt"));
            Assert.That(duplicateResult.GetUnit(0).LineNumber, Is.EqualTo(40));

            //when
            duplicateResult.AddUnit("2.txt", 120);
            //then
            Assert.That(duplicateResult.GetUnitsCount(), Is.EqualTo(4));
            Assert.That(duplicateResult.GetUnit(2).FileName, Is.EqualTo("2.txt"));
            Assert.That(duplicateResult.GetUnit(2).LineNumber, Is.EqualTo(120));
        }

        #region Compare Tests

        [Test]
        public void CompareEqual()
        {
            //given
            var duplicateResult1 = new DuplicateResult(10);
            duplicateResult1.AddUnit("1.txt", 5);
            duplicateResult1.AddUnit("2.txt", 6);
            var duplicateResult2 = new DuplicateResult(10);
            duplicateResult2.AddUnit("1.txt", 5);
            duplicateResult2.AddUnit("2.txt", 6);
            //then
            Assert.IsTrue(duplicateResult1.CompareTo(duplicateResult2) == 0);
        }

        [Test]
        public void CompareDifferentLinesCount()
        {
            //given
            var duplicateResult1 = new DuplicateResult(5);
            duplicateResult1.AddUnit("1.txt", 5);
            duplicateResult1.AddUnit("2.txt", 6);
            duplicateResult1.AddUnit("3.txt", 144);
            var duplicateResult2 = new DuplicateResult(10);
            duplicateResult2.AddUnit("1.txt", 5);
            duplicateResult2.AddUnit("2.txt", 6);
            //then
            Assert.That(duplicateResult1.CompareTo(duplicateResult2), Is.EqualTo(-1));
            Assert.That(duplicateResult2.CompareTo(duplicateResult1), Is.EqualTo(1));
        }

        [Test]
        public void CompareDifferentUnitsCount()
        {
            //given
            var duplicateResult1 = new DuplicateResult(10);
            duplicateResult1.AddUnit("1.txt", 5);
            duplicateResult1.AddUnit("2.txt", 6);
            duplicateResult1.AddUnit("3.txt", 144);
            var duplicateResult2 = new DuplicateResult(10);
            duplicateResult2.AddUnit("1.txt", 5);
            duplicateResult2.AddUnit("2.txt", 6);
            //then
            Assert.That(duplicateResult1.CompareTo(duplicateResult2), Is.EqualTo(1));
            Assert.That(duplicateResult2.CompareTo(duplicateResult1), Is.EqualTo(-1));
        }

        [Test]
        public void CompareDifferentFileNames()
        {
            //given
            var duplicateResult1 = new DuplicateResult(10);
            duplicateResult1.AddUnit("1.txt", 5);
            duplicateResult1.AddUnit("2.txt", 6);
            var duplicateResult2 = new DuplicateResult(10);
            duplicateResult2.AddUnit("2.txt", 5);
            duplicateResult2.AddUnit("3.txt", 6);
            //then
            Assert.That(duplicateResult1.CompareTo(duplicateResult2), Is.EqualTo(-1));
            Assert.That(duplicateResult2.CompareTo(duplicateResult1), Is.EqualTo(1));
        }

        [Test]
        public void CompareDifferentLineNumbers()
        {
            //given
            var duplicateResult1 = new DuplicateResult(10);
            duplicateResult1.AddUnit("1.txt", 5);
            duplicateResult1.AddUnit("2.txt", 6);
            var duplicateResult2 = new DuplicateResult(10);
            duplicateResult2.AddUnit("1.txt", 15);
            duplicateResult2.AddUnit("2.txt", 66);
            //then
            Assert.That(duplicateResult1.CompareTo(duplicateResult2), Is.EqualTo(-1));
            Assert.That(duplicateResult2.CompareTo(duplicateResult1), Is.EqualTo(1));
        }

        #endregion
    }
}
