using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.CodeClasses;
using NUnit.Framework;

namespace UnitTests.BusinessLayer.CodeClasses
{
    [TestFixture]
    public class MethodParserTests
    {
        [Test]
        public void MethodParserTest()
        {
            var methodParser = new MethodParser("   List<string> GetSomeValue(int n, string str)  ");
            Assert.IsTrue(methodParser.IsValid);
            Assert.AreEqual("List<string>", methodParser.ReturnType);
            Assert.AreEqual("GetSomeValue", methodParser.MethodName);
            Assert.AreEqual("int n, string str", methodParser.Parameters);
            
            methodParser = new MethodParser("\tint GetSomeAnotherValue()\t  ");
            Assert.IsTrue(methodParser.IsValid);
            Assert.AreEqual("int", methodParser.ReturnType);
            Assert.AreEqual("GetSomeAnotherValue", methodParser.MethodName);
            Assert.AreEqual(String.Empty, methodParser.Parameters);
            
            methodParser = new MethodParser("List<string> ololo GetSomeValue(int n, string str)");
            Assert.IsFalse(methodParser.IsValid);
            methodParser = new MethodParser("List<string> GetSomeValue");
            Assert.IsFalse(methodParser.IsValid);
            methodParser = new MethodParser("List<string> GetSomeValue(int n, string str");
            Assert.IsFalse(methodParser.IsValid);
            methodParser = new MethodParser("List<string> GetSomeValue)");
            Assert.IsFalse(methodParser.IsValid);
        }
    }
}
