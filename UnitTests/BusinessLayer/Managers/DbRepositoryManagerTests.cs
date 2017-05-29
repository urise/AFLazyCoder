using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Managers;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.Managers
{
    [TestFixture]
    public class DbRepositoryManagerTests
    {
        [Test]
        public void FilterDbRepositoriesTest()
        {
            string[] fileList = 
                {"SomeFile", "SomeDbRepository", "SecondDbRepository.cs", 
                 "DbRepository.cs", "DbRepository.csproj", @"d:\work\AF\FirstDbRepository.cs"};
            List<string> result = DbRepositoryManager.FilterClasses(fileList);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("FirstDbRepository", result[0]);
            Assert.AreEqual("SecondDbRepository", result[1]);
        }

        [Test]
        public void GetInterfaceFileNameTest()
        {
            var manager = new DbRepositoryManager();
            manager.Init(@"d:\AF\Source");
            Assert.AreEqual(@"d:\AF\Source\IFS.Interfaces\DbRepository\IUserDbRepository.cs",
                manager.GetInterfaceFileName("UserDbRepository"));
        }

        [Test]
        public void GetDbRepositoryFileNameTest()
        {
            var manager = new DbRepositoryManager();
            manager.Init(@"d:\AF\Source");
            Assert.AreEqual(@"d:\AF\Source\IFS.DbRepository\UserDbRepository.cs",
                manager.GetDbRepositoryFileName("UserDbRepository"));
        }

        [Test]
        public void GetMockDbRepositoryFileNameTest()
        {
            var manager = new DbRepositoryManager();
            manager.Init(@"d:\AF\Source");
            Assert.AreEqual(@"d:\AF\Source\IFS.NUnitTests\DbRepository\MockUserDbRepository.cs",
                manager.GetMockDbRepositoryFileName("UserDbRepository"));
        }
    }
}
