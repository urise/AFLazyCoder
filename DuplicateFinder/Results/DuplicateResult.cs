using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using BusinessLayer.AnalyzeResults;

namespace AFDuplicateFinder.Results
{
    public class DuplicateResult: IAnalyzeResult
    {
        #region Fields and Properties

        public int LinesCount { get; private set; }

        private List<DuplicateResultUnit> _units = new List<DuplicateResultUnit>();

        public DuplicateResult(int linesCount = 0)
        {
            LinesCount = linesCount;
        }

        #endregion

        #region Methods

        private int GetIndexForInsertUnit(string fileName, int lineNumber)
        {
            for (int i = 0; i < _units.Count; i++)
            {
                var unit = _units[i];
                if (string.CompareOrdinal(unit.FileName, fileName) > 0 || unit.FileName == fileName && unit.LineNumber > lineNumber)
                    return i;
            }
            return -1;
        }

        public void AddUnit(string fileName, int lineNumber)
        {
            var index = GetIndexForInsertUnit(fileName, lineNumber);
            var unit = new DuplicateResultUnit(fileName, lineNumber);

            if (index == -1) _units.Add(unit);
            else _units.Insert(index, unit);
        }

        public int GetUnitsCount()
        {
            return _units.Count;
        }

        public DuplicateResultUnit GetUnit(int n)
        {
            return _units[n];
        }

        private int GetLineNumberInsideUnit(int unitNumber, string fileName, int lineNumber)
        {
            var unit = GetUnit(unitNumber);
            if (unit.FileName == fileName && lineNumber - unit.LineNumber < LinesCount)
            {
                return lineNumber - unit.LineNumber + 1;
            }
            return -1;
        }

        public bool IsPartOfThisResult(PairOfPlaces pairOfPlaces)
        {
            for (int i = 0; i < GetUnitsCount(); i++)
            {
                var lineNumberInsideUnit1 = GetLineNumberInsideUnit(i, pairOfPlaces.FileName1, pairOfPlaces.LineNumber1);
                if (lineNumberInsideUnit1 == -1) continue;
                for (int j = 0; j < GetUnitsCount(); j++)
                {
                    var lineNumberInsideUnit2 = GetLineNumberInsideUnit(j, pairOfPlaces.FileName2, pairOfPlaces.LineNumber2);
                    if (lineNumberInsideUnit1 == lineNumberInsideUnit2) return true;
                }
            }
            return false;
        }

        public bool IsIncludedInThisResult(int linesCount, string fileName, int lineNumber)
        {
            return LinesCount == linesCount && 
                _units.Any(r => r.FileName == fileName && r.LineNumber == lineNumber);
        }

        #endregion

        #region Xml Methods

        public XmlNode GetXmlNode(XmlDocument xmlDocument)
        {
            var xmlNode = xmlDocument.CreateElement("result");
            var attribute = xmlDocument.CreateAttribute("linescount");
            attribute.Value = LinesCount.ToString();
            xmlNode.Attributes.Append(attribute);
            
            foreach(var unit in _units)
            {
                xmlNode.AppendChild(unit.GetXmlNode(xmlDocument));
            }
            return xmlNode;
        }

        public void InitFromXmlNode(XmlNode node)
        {
            LinesCount = int.Parse(node.Attributes["linescount"].Value);
            XmlNodeList nodes = node.SelectNodes("unit");
            _units.Clear();
            if (nodes == null) return;
            foreach (XmlNode unitNode in nodes)
            {
                var unit = new DuplicateResultUnit(unitNode);
                _units.Add(unit);
            }
        }

        #endregion

        public int CompareTo(IAnalyzeResult other)
        {
            if (! (other is DuplicateResult)) 
                throw new Exception("DuplicateResult can be comoparable only with another DuplicateResult");
            var otherDuplicateResult = other as DuplicateResult;

            if (LinesCount < otherDuplicateResult.LinesCount) return -1;
            if (LinesCount > otherDuplicateResult.LinesCount) return 1;
            if (GetUnitsCount() < otherDuplicateResult.GetUnitsCount()) return -1;
            if (GetUnitsCount() > otherDuplicateResult.GetUnitsCount()) return 1;
            
            for (int i = 0; i < GetUnitsCount(); i++)
            {
                int compareFileNamesResult = String.Compare(GetUnit(i).FileName, otherDuplicateResult.GetUnit(i).FileName, StringComparison.Ordinal);
                if (compareFileNamesResult != 0) return compareFileNamesResult;
                if (GetUnit(i).LineNumber < otherDuplicateResult.GetUnit(i).LineNumber) return -1;
                if (GetUnit(i).LineNumber > otherDuplicateResult.GetUnit(i).LineNumber) return 1;
            }
            return 0;
        }

        public int CompareTo(object obj)
        {
            if (!(obj is IAnalyzeResult))
                throw new Exception("obj should be IAnalyzeResult");
            return CompareTo((IAnalyzeResult)obj);
        }

        public string ToHtml()
        {
            var result = new StringBuilder();
            result.AppendLine("<b>Lines Count: " + LinesCount + "</b>");
            for (int i = 0; i < GetUnitsCount(); i++)
            {
                var unit = GetUnit(i);
                result.AppendLine("<br>" + unit.LineNumber + ": " + unit.FileName);
            }
            result.AppendLine("<br>");
            return result.ToString();
        }
    }
}
