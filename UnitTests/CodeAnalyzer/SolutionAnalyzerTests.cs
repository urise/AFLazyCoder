using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeAnalyzer;
using CodeAnalyzer.CodeData;
using NUnit.Framework;

namespace UnitTests.CodeAnalyzer
{
    [TestFixture]
    public class SolutionAnalyzerTests
    {
        [Test]
        public void AnalyzeComplexTest()
        {
            var solutionAnalyzer = new SolutionAnalyzer();
            var solutionData = solutionAnalyzer.Analyze(@"..\..\CodeAnalyzer\TestData\ApplicationForAnalysis\ApplicationForAnalysis.sln");
            Assert.AreEqual("ApplicationForAnalysis.sln", solutionData.SolutionFileName);
            Assert.AreEqual("ApplicationForAnalysis", solutionData.SolutionName);
            Assert.AreEqual(@"..\..\CodeAnalyzer\TestData\ApplicationForAnalysis", solutionData.SolutionFolderName);

            Assert.AreEqual(3, solutionData.Projects.Count);
            Assert.AreEqual("WinFormProject", solutionData.Projects[0].ProjectName);
            Assert.AreEqual(@"WinFormProject\WinFormProject.csproj", solutionData.Projects[0].ProjectFileName);
            Assert.AreEqual("WinFormProject", solutionData.Projects[0].ProjectFolder);

            Assert.AreEqual("ClassLibraryProject", solutionData.Projects[1].ProjectName);
            Assert.AreEqual(@"ClassLibraryProject\ClassLibraryProject.csproj", solutionData.Projects[1].ProjectFileName);
            Assert.AreEqual("ClassLibraryProject", solutionData.Projects[1].ProjectFolder);

            Assert.AreEqual("WebProject", solutionData.Projects[2].ProjectName);
            Assert.AreEqual(@"WebProject\WebProject.csproj", solutionData.Projects[2].ProjectFileName);
            Assert.AreEqual("WebProject", solutionData.Projects[2].ProjectFolder);
        }

        [Test]
        public void ParseProjectLineTest()
        {
            ProjectData result = SolutionAnalyzer.ParseProjectLine("some not interesting string");
            Assert.IsNull(result);
            result = SolutionAnalyzer.ParseProjectLine("Project(\"{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC\") = \"ClassLibraryProject\", \"ClassLibraryProject\\ClassLibraryProject.csproj\", \"{953E1F18-3833-4CEB-8F51-9338ECB1E0F6}\"");
            Assert.AreEqual("ClassLibraryProject", result.ProjectName);
            Assert.AreEqual(@"ClassLibraryProject\ClassLibraryProject.csproj", result.ProjectFileName);
            Assert.AreEqual("ClassLibraryProject", result.ProjectFolder);
        }

        //[Test]
        //public void AnalyzeExceptionTest()
        //{
        //    var solutionAnalyzer = new SolutionAnalyzer();
        //    Assert.<solutionAnalyzer.Analyze("_non_existing_file_")>();

        //}
    }
}
