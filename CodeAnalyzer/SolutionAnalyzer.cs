using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using CodeAnalyzer.CodeData;

namespace CodeAnalyzer
{
    public class SolutionAnalyzer
    {
        private SolutionData _solutionData;

        public SolutionData Analyze(string solutionFileName)
        {
            if (!File.Exists(solutionFileName))
                throw new Exception("File " + solutionFileName + " is not found.");
            _solutionData = new SolutionData();
            _solutionData.SolutionFileName = Path.GetFileName(solutionFileName);
            _solutionData.SolutionName = Path.GetFileNameWithoutExtension(solutionFileName);
            _solutionData.SolutionFolderName = Path.GetDirectoryName(solutionFileName);
            _solutionData.Projects = GetProjectListFromSolutionFile(solutionFileName);
            return _solutionData;
        }

        private static List<ProjectData> GetProjectListFromSolutionFile(string solutionFileName)
        {
            var result = new List<ProjectData>();
            List<string> lines = File.ReadAllLines(solutionFileName).ToList();
            foreach(string line in lines)
            {
                var projectData = ParseProjectLine(line);
                if (projectData != null)
                    result.Add(projectData);
            }
            return result;
        }

        public static ProjectData ParseProjectLine(string solutionLine)
        {
            const string pattern = @"Project\(""\S+""\)\s=\s""(\S+)"",\s""(\S+)"",\s""\S+""";
            var regex = new Regex(pattern);
            var match = regex.Match(solutionLine);
            if (!match.Success) return null;
            
            var result = new ProjectData();
            result.ProjectName = match.Groups[1].ToString();
            result.ProjectFileName = match.Groups[2].ToString();
            result.ProjectFolder = Path.GetDirectoryName(result.ProjectFileName);
            return result;

        }
    }
}
