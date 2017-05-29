using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.CodeClasses;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.CodeClasses
{
    [TestFixture]
    public class CodeHelperTests
    {
        [Test]
        public void GetInterfaceTest()
        {
            const string methodSignature = "List<string> GetSomeMethod(int n, string str)";
            const string expected = "List<string> GetSomeMethod(int n, string str);";
            var method = new MethodParser(methodSignature);
            List<string> result = CodeHelper.GetInterfaceSignatureForMethod(method);
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual(expected, result[0]);
        }

        [Test]
        public void GetValidReturnStatementTest()
        {
            Assert.AreEqual("return;", CodeHelper.GetValidReturnStatement("void"));
            Assert.AreEqual("return 0;", CodeHelper.GetValidReturnStatement("int"));
            Assert.AreEqual("return null;", CodeHelper.GetValidReturnStatement("List<string>"));
            Assert.AreEqual("return null;", CodeHelper.GetValidReturnStatement("ololo"));
        }

        [Test]
        public void GetEmptyMethodTest()
        {
            const string methodSignature = "List<string> GetSomeMethod(int n, string str)";
            var method = new MethodParser(methodSignature);

            List<string> result = CodeHelper.GetEmptyMethod(method);
            Assert.AreEqual(5, result.Count);
            Assert.IsEmpty(result[0]);
            Assert.AreEqual("public List<string> GetSomeMethod(int n, string str)", result[1]);
            Assert.AreEqual("{", result[2]);
            Assert.AreEqual("    return null;", result[3]);
            Assert.AreEqual("}", result[4]);
        }

        [Test]
        public void GetMockMethodTest()
        {
            const string methodSignature = "List<string> GetSomeMethod(int n, string str)";
            var method = new MethodParser(methodSignature);

            List<string> result = CodeHelper.GetMockMethod(method, "SomeDbRepository");
            Assert.AreEqual(5, result.Count);
            Assert.IsEmpty(result[0]);
            Assert.AreEqual("public List<string> GetSomeMethod(int n, string str)", result[1]);
            Assert.AreEqual("{", result[2]);
            Assert.AreEqual("    return (List<string>)MockResultSingleton.GetResult(\"SomeDbRepository.GetSomeMethod\");", result[3]);
            Assert.AreEqual("}", result[4]);
        }

        [Test]
        public void FindPlaceInFileTests()
        {
            var classCode = 
                new List<string>
                {
                    "namespace ololo",
                    "{",
                    "    class blabla",
                    "    {",
                    "        void method ogogo()",
                    "        {",
                    "             return;",
                    "        }",
                    "    }",
                    "}"
                };

            PlaceInFile place = CodeHelper.FindPlaceForMethod(classCode);
            Assert.AreEqual(8, place.LineNumber);
            Assert.AreEqual("        ", place.Spaces);
        }

        [Test]
        public void GetEmptyInterfaceTest()
        {
            List<string> result = CodeHelper.GetEmptyInterface("ISomeInterface", "ISome1, ISome2");
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("public interface ISomeInterface: ISome1, ISome2", result[0]);
            Assert.AreEqual("{", result[1]);
            Assert.AreEqual("}", result[2]);
        }

        [Test]
        public void IndentAllLinesIntTest()
        {
            var list = new List<string> {"abcde", "fgh 123", "  01"};
            List<string> result = list.IndentAllLines(4);
            Assert.AreEqual(3, result.Count);
            Assert.AreEqual("    abcde", result[0]);
            Assert.AreEqual("    fgh 123", result[1]);
            Assert.AreEqual("      01", result[2]);
        }

        [Test]
        public void IndentAllLinesstringTest()
        {
            var list = new List<string> { "abcde", "fgh 123", "  01" };
            List<string> result = list.IndentAllLines("  ");
            var expected = new List<string> { "  abcde", "  fgh 123", "    01" };
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void AddNamespaceTest()
        {
            var code = new List<string>
                           {
                               "public class SomeClass",
                               "{",
                               "    private int _number;",
                               "}"
                           };
            var expected = new List<string>
                               {
                                   "namespace Some.FunnyNamespace",
                                   "{",
                                   "    public class SomeClass",
                                   "    {",
                                   "        private int _number;",
                                   "    }",
                                   "}"
                               };
            List<string> result = CodeHelper.AddNamespace(code, "Some.FunnyNamespace");
            CollectionAssert.AreEqual(expected, result);
        }

        [Test]
        public void FindLastLineThatContainTextTest()
        {
            var code = new List<string>
                           {
                               "<Project>",
                               "  <ItemGroup>",
                               "    <Compile Include=\"File1.cs\"",
                               "    <Compile Include=\"File2.cs\"",
                               "    <Compile Include=\"File3.cs\"",
                               "  </ItemGroup>",
                               "</Project>"
                           };
            PlaceInFile result = CodeHelper.FindLastLineThatContainText(code, "<Compile Include");
            Assert.AreEqual(4, result.LineNumber);
            Assert.AreEqual("    ", result.Spaces);
        }

        [Test]
        public void GetSpringConfigLineTest()
        {
            Assert.AreEqual("<object id=\"UserDbRepository\" type=\"IFS.NUnitTests.DbRepository.MockUserDbRepository, IFS.NUnitTests\" singleton=\"false\"/>", 
                CodeHelper.GetSpringConfigLine("UserDbRepository", "IFS.NUnitTests", "DbRepository.MockUserDbRepository", false));
            Assert.AreEqual("<object id=\"OrganizationDbRepository\" type=\"IFS.DbRepository.OrganizationDbRepository, IFS.DbRepository\"/>",
                CodeHelper.GetSpringConfigLine("OrganizationDbRepository", "IFS.DbRepository", "OrganizationDbRepository", true));
        }
    }
}
