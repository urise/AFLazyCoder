using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace AFDuplicateFinder.Results
{
    public class DuplicateResultFull: IEnumerable<DuplicateResult>
    {
        #region Fields and Properties

        private readonly List<DuplicateResult> _results = new List<DuplicateResult>();

        public int Count { get { return _results.Count; } }

        public DuplicateResult this[int index] { get { return _results[index]; } }

        #endregion

        #region Methods

        public void Add(DuplicateResult duplicateResult)
        {
            _results.Add(duplicateResult);
        }

        public void Add(int numberOfLines, FileLine fileLine1, FileLine fileLine2)
        {
            var duplicateResultIndex = GetExistingResultIndex(numberOfLines, fileLine1);
            DuplicateResult duplicateResult;
            if (duplicateResultIndex == -1)
            {
                duplicateResult = new DuplicateResult(numberOfLines);
                duplicateResult.AddUnit(fileLine1.FileName, fileLine1.LineNumber);
                duplicateResult.AddUnit(fileLine2.FileName, fileLine2.LineNumber);
                Add(duplicateResult);
                duplicateResultIndex = Count - 1;
            }
            else
            {
                duplicateResult = _results[duplicateResultIndex];
                if (!duplicateResult.IsIncludedInThisResult(numberOfLines, fileLine2.FileName, fileLine2.LineNumber))
                    duplicateResult.AddUnit(fileLine2.FileName, fileLine2.LineNumber);
            }
            fileLine1.AddResult(duplicateResultIndex);
            fileLine2.AddResult(duplicateResultIndex);
        }

        public bool IsPartOfAnotherResult(PairOfPlaces pairOfPlaces)
        {
            return _results.Any(r => r.IsPartOfThisResult(pairOfPlaces));
        }

        public int GetExistingResultIndex(int numberOfLines, FileLine fileLine)
        {
            for (int i = 0; i < fileLine.GetResultsCount(); i++)
            {
                var resultIndex = fileLine.GetResultIndex(i);
                var duplicateResult = _results[resultIndex];
                if (duplicateResult.IsIncludedInThisResult(numberOfLines, fileLine.FileName, fileLine.LineNumber))
                    return resultIndex;
            }
            return -1;
        }

        public string GetTextReport()
        {
            var report = new StringBuilder(_results.Count*200);
            foreach(var duplicateResult in _results.OrderByDescending(r => r.LinesCount).ThenByDescending(r => r.GetUnitsCount()))
            {
                report.AppendLine("======================================================");
                report.AppendLine("Lines Count: " + duplicateResult.LinesCount);
                for (int i = 0; i < duplicateResult.GetUnitsCount(); i++)
                {
                    var unit = duplicateResult.GetUnit(i);
                    report.AppendLine("\t" + unit.LineNumber + ": " + unit.FileName);
                }
            }
            return report.ToString();
        }

        public string GetHtmlReport()
        {
            var report = new StringBuilder();
            foreach (var duplicateResult in _results.OrderByDescending(r => r.LinesCount).ThenByDescending(r => r.GetUnitsCount()))
            {
                report.AppendLine(duplicateResult.ToHtml());
            }
            report.AppendLine("<br>");
            return report.ToString();
        }

        #endregion

        #region Xml Methods

        public string ToXml()
        {
            var xmlDocument = new XmlDocument();
            var element = xmlDocument.CreateElement("results");
            xmlDocument.AppendChild(element);

            foreach (var duplicateResult in this)
            {
                element.AppendChild(duplicateResult.GetXmlNode(xmlDocument));
            }
            return xmlDocument.InnerXml;
        }

        public void InitFromXml(string xml)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xml);
            if (xmlDocument.DocumentElement == null)
                throw new Exception("Wrong xml format");
            XmlNodeList nodes = xmlDocument.DocumentElement.SelectNodes("/results/result");
            
            _results.Clear();
            foreach (XmlNode node in nodes)
            {
                var duplicateResult = new DuplicateResult();
                duplicateResult.InitFromXmlNode(node);
                _results.Add(duplicateResult);
            }
        }

        #endregion

        #region IEnumerable Members

        public IEnumerator<DuplicateResult> GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _results.GetEnumerator();
        }

        #endregion
    }
}
