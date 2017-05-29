using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.InfoClasses.Common;
using NUnit.Framework;

namespace UnitTests.InfoClasses.Common
{
    [TestFixture]
    public class ProgramParametersTests
    {
        [Test]
        public void TestNoParameters()
        {
            var programParameters = new ProgramParameters();
            programParameters.Init(new string[]{});
            Assert.IsEmpty(programParameters.ToList());
        }

        [Test]
        public void TestOneSimpleParameter()
        {
            var programParameters = new ProgramParameters();
            programParameters.Init(new [] {"-abc"});
            var result = programParameters.ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("abc", result[0].Command);
            Assert.AreEqual(0, result[0].Arguments.Count);
        }

        [Test]
        public void TestOneParameterWithArguments()
        {
            var programParameters = new ProgramParameters();
            programParameters.Init(new [] { "-abc", "first", "second", "third"});
            var result = programParameters.ToList();
            Assert.AreEqual(1, result.Count);
            Assert.AreEqual("abc", result[0].Command);
            Assert.AreEqual(3, result[0].Arguments.Count);
            Assert.AreEqual("first", result[0].Arguments[0]);
            Assert.AreEqual("second", result[0].Arguments[1]);
            Assert.AreEqual("third", result[0].Arguments[2]);
        }

        [Test]
        public void TestSeveralParameterWithArguments()
        {
            var programParameters = new ProgramParameters();
            programParameters.Init(new [] { "-abc", "first", "-d", "one more", "second", "-e", "-fg" });
            var result = programParameters.ToList();
            Assert.AreEqual(4, result.Count);
            
            Assert.AreEqual("abc", result[0].Command);
            Assert.AreEqual(1, result[0].Arguments.Count);
            Assert.AreEqual("first", result[0].Arguments[0]);
            
            Assert.AreEqual("d", result[1].Command);
            Assert.AreEqual(2, result[1].Arguments.Count);
            Assert.AreEqual("one more", result[1].Arguments[0]);
            Assert.AreEqual("second", result[1].Arguments[1]);

            Assert.AreEqual("e", result[2].Command);
            Assert.AreEqual(0, result[2].Arguments.Count);

            Assert.AreEqual("fg", result[3].Command);
            Assert.AreEqual(0, result[3].Arguments.Count);
        }

        [Test]
        public void TestBadParameters()
        {
            var programParameters = new ProgramParameters();
            var ex = Assert.Throws<Exception>(() => programParameters.Init(new[] {"abc"}));
            Assert.AreEqual("Bad program parameters: command is expected", ex.Message);
        }
    }
}
