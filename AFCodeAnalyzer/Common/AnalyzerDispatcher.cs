using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using AFDuplicateFinder;
using AFDuplicateFinder.LanguageConfigurations;
using AFDuplicateFinder.Results;
using BusinessLayer.CodeAnalyzer.Searching;
using BusinessLayer.Common;
using BusinessLayer.Helpers;
using BusinessLayer.InfoClasses.Common;

namespace AFCodeAnalyzer.Common
{
    public class AnalyzerDispatcher
    {
        #region Enums

        private enum RevisionGetMethod
        {
            UseSvnInfo,
            Incremental
        }

        #endregion

        #region Fields and Properties

        private readonly string _workingFolder;
        private readonly string _htmlReportFolder;
        private readonly string _executeType;
        private readonly string _executeConfig;
        private readonly int _reportDepthLimit = 20;
        private readonly RevisionGetMethod _revisionGetMethod = RevisionGetMethod.UseSvnInfo;

        private string CommitsFileName
        {
            get { return Path.Combine(_workingFolder, "commits"); }
        }

        private List<string> _commitsList; 
        private List<string> CommitsList
        {
            get
            {
                if (_commitsList != null) return _commitsList;
                if (!File.Exists(CommitsFileName))
                {
                    File.WriteAllText(CommitsFileName, string.Empty);
                    _commitsList = new List<string>();
                }
                else
                    _commitsList = File.ReadAllLines(CommitsFileName).Distinct().ToList();
                return _commitsList;
            }
        }

        public AnalyzerDispatcher() { }

        public AnalyzerDispatcher(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);

            var node = xmlDocument.SelectSingleNode("/root/WorkingFolder");
            if (node == null) throw new Exception("WorkingFolder node is not found in xml config");
            _workingFolder = node.InnerText;

            node = xmlDocument.SelectSingleNode("/root/HtmlReportFolder");
            if (node == null) throw new Exception("HtmlReportFolder node is not found in xml config");
            _htmlReportFolder = node.InnerText;

            node = xmlDocument.SelectSingleNode("/root/ReportDepthLimit");
            if (node != null && !int.TryParse(node.InnerText, out _reportDepthLimit))
                throw new Exception("ReportDepthLimit should be integer in xml config");

            node = xmlDocument.SelectSingleNode("/root/ExecuteType");
            if (node == null) throw new Exception("ExecuteType node is not found in xml config");
            _executeType = node.InnerText;

            node = xmlDocument.SelectSingleNode("/root/ExecuteConfig");
            if (node == null) throw new Exception("ExecuteConfig node is not found in xml config");
            _executeConfig = node.InnerText;

            node = xmlDocument.SelectSingleNode("/root/RevisionGetMethod");
            if (node != null && !Enum.TryParse(node.InnerText, true, out _revisionGetMethod)) 
                throw new Exception("RevisionGetMethod node is invalid in xml config");
        }

        #endregion

        #region Methods

        private int GetCurrentSvnRevision(string directory)
        {
            switch (_revisionGetMethod)
            {
                case RevisionGetMethod.UseSvnInfo:
                    return SvnHelper.GetCurrentRevision(directory);
                case RevisionGetMethod.Incremental:
                    return GetPreviousSvnRevision() + 1;
                default:
                    throw new Exception();
            }
        }

        private int GetPreviousSvnRevision()
        {
            return CommitsList.Count == 0 ? 0 : int.Parse(CommitsList[CommitsList.Count - 1]);
        }

        private void CreateHtmlReport(DuplicateResultFull lastResultFull)
        {
            if (!Directory.Exists(_htmlReportFolder)) Directory.CreateDirectory(_htmlReportFolder);
            File.WriteAllText(Path.Combine(_htmlReportFolder, "fullduplicates.html"), lastResultFull.GetHtmlReport());
            var indexHtml = new StringBuilder();
            indexHtml.AppendLine("<html>");
            indexHtml.AppendLine("<head><title>Duplicate Reports</title>");
            indexHtml.AppendLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\" />");
            indexHtml.AppendLine("</head>");
            indexHtml.AppendLine("<body>");
            indexHtml.AppendLine("<h2>REPORT</h2>");
            indexHtml.AppendLine("<a href=\"fullduplicates.html\">Full duplicates report</a><br>");

            indexHtml.AppendLine("<table><tr><th>Revision</th><th>Plus</th><th>Minus</th></tr>");
            DuplicateResultFull previousResult = lastResultFull;
            DuplicateResultFull currentResult;
            int counter = 0;
            for (int i = CommitsList.Count - 2; i >= 0; i--)
            {
                if (++counter > _reportDepthLimit) break;
                currentResult = previousResult;
                previousResult = new DuplicateResultFull();
                var xmlFileName = Path.Combine(_workingFolder, CommitsList[i].ToString(), "duplicate.xml");
                var xml = File.ReadAllText(xmlFileName);
                previousResult.InitFromXml(xml);
                var comparison = new DuplicateResultComparison(previousResult, currentResult);
                string comparisonFileName = "comparison" + CommitsList[i] + "-" + CommitsList[i + 1] + ".html";
                File.WriteAllText(Path.Combine(_htmlReportFolder, comparisonFileName), comparison.GetHtmlReport());

                indexHtml.AppendLine("<tr><td><a href=\"" + comparisonFileName + "\">" + CommitsList[i+1] + "</a></td>");
                indexHtml.AppendLine("<td>+" + comparison.PlusCount + "</td>");
                indexHtml.AppendLine("<td>-" + comparison.MinusCount + "</td></tr>");
            }
            indexHtml.AppendLine("</table>");
            indexHtml.AppendLine("</body>");
            indexHtml.AppendLine("</html>");
            File.WriteAllText(Path.Combine(_htmlReportFolder, "index.html"), indexHtml.ToString());
        }

        private void ExecuteSearchDuplicates(CodeAnalyzerParameters parameters)
        {
            if (!Directory.Exists(_workingFolder)) Directory.CreateDirectory(_workingFolder);
            int svnRevision = GetCurrentSvnRevision(parameters.Directory);
            var language = new CSharpLanguage();
            var fileContents = GetFolderContents(parameters.Directory, parameters.SearchFilter, language.PreliminaryProcess);
            var duplicateFindParameters = new DuplicateFindParameters(6, new CSharpLanguage());
            var filesCombiner = new DuplicateFinder(fileContents, duplicateFindParameters);
            var result = filesCombiner.FindDuplicates();
            var folder = Path.Combine(_workingFolder, svnRevision.ToString());
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            if (!CommitsList.Contains(svnRevision.ToString()))
            {
                File.AppendAllText(CommitsFileName, svnRevision + "\n");
                CommitsList.Add(svnRevision.ToString());
            }
            var reportFileName = Path.Combine(folder, "duplicate.xml");
            File.WriteAllText(reportFileName, result.ToXml());

            var previousReport = Path.Combine(_workingFolder, GetPreviousSvnRevision().ToString(),
                "duplicate.xml");

            Console.WriteLine("Lines: " + result.Count);
            CreateHtmlReport(result);
        }

        public void ExecuteOther(CodeAnalyzerParameters parameters)
        {
            var textSearcher = new TextSearcher(parameters.GetExtractFunction());
            textSearcher.ProcessFolder(parameters.Directory, parameters.SearchFilter);
            foreach (var report in parameters.Reports)
            {
                var result = textSearcher.GetFormattedReport(report);
                File.WriteAllLines(report.OutputFileName, result);
            }
        }

        public void Execute()
        {
            try
            {
                var parameters = GetParametersFromXml(_executeConfig);

                if (parameters.Command == CodeAnalyzerCommand.SearchDuplicates)
                {
                    ExecuteSearchDuplicates(parameters);
                }
                else
                {
                    ExecuteOther(parameters);
                }
                Console.WriteLine("Program completed");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                if (!(ex is CustomException))
                    Console.WriteLine(ex.StackTrace);
            }
        }

        private static CodeAnalyzerParameters GetParametersFromXml(string configFileName)
        {
            if (!File.Exists(configFileName))
                throw new Exception("xml config does not exist");
            var serializer = new XmlSerializer(typeof(CodeAnalyzerParameters), new Type[] { });
            using (var reader = File.OpenRead(configFileName))
            {
                return (CodeAnalyzerParameters)serializer.Deserialize(reader);
            }
        }

        public static List<FileContent> GetFolderContents(string baseDirectoryName, Func<string, bool> searchFilter,
            Func<IEnumerable<string>, IEnumerable<string>> preliminaryProcess)
        {
            var fileList = Directory.GetFiles(baseDirectoryName, "*.*", SearchOption.AllDirectories).Where(searchFilter);
            var result = new List<FileContent>();
            foreach (var fileName in fileList)
            {
                result.Add(new FileContent(fileName, preliminaryProcess(File.ReadAllLines(fileName))));
            }
            return result;
        }

        #endregion
    }
}
