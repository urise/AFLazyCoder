using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BusinessLayer.Helpers;

namespace BusinessLayer.CodeAnalyzer.Searching
{
    public enum TextSearcherReportOrder
    {
        Quantity,
        Length
    }

    public class TextSearcher
    {
        #region Fields and Properties
        
        private Dictionary<string, List<PlaceInfo>> _dict = new Dictionary<String, List<PlaceInfo>>();
        private Func<string, List<string>> ExtractStrings { get; set; }

        #endregion

        public TextSearcher(Func<string, List<string>> func)
        {
            ExtractStrings = func;
        }

        public void ProcessFile(string fileName, int baseDirectoryLength)
        {
            var lines = File.ReadAllLines(fileName);
            int lineNumber = 1;
            foreach (var line in lines)
            {
                var literals = ExtractStrings(line);
                foreach (var literal in literals)
                    AddInfo(literal, fileName.Substring(baseDirectoryLength), lineNumber);
                lineNumber++;
            }
        }

        public void ProcessFolder(string baseDirectoryName, Func<string, bool> searchFilter)
        {
            var fileList = Directory.GetFiles(baseDirectoryName, "*.*", SearchOption.AllDirectories).Where(searchFilter);
            foreach (var fileName in fileList)
            {
                ProcessFile(fileName, baseDirectoryName.Length + (baseDirectoryName.EndsWith(@"\") ? 0 : 1));
            }
        }

        public void AddInfo(string literal, string fileName, int lineNumber)
        {
            List<PlaceInfo> list;
            if (!_dict.TryGetValue(literal, out list))
            {
                _dict.Add(literal, list = new List<PlaceInfo>());
            }
            list.Add(new PlaceInfo(fileName, lineNumber));
        }

        public List<string> GetFormattedReport(SearchReportParameters reportParameters)
        {
            var result = new List<string>();
            var list = _dict.Keys.Select(r =>
                                         new
                                         {
                                             Literal = r,
                                             Value = _dict[r]
                                         });

            list = list.Where(r => r.Value.Count >= reportParameters.QuantityLimit);
            switch (reportParameters.Order)
            {
                case TextSearcherReportOrder.Quantity:
                    list = list.OrderByDescending(r => r.Value.Count);
                    break;
                case TextSearcherReportOrder.Length:
                    list = list.OrderByDescending(r => r.Literal.Length).ThenByDescending(r => r.Value.Count);
                    break;
            }
            foreach (var v in list)
            {
                result.Add(string.Format("\"{0}\": {1}", v.Literal, v.Value.Count));
                if (reportParameters.IsDetailed)
                {
                    result.AddRange(GetFormattedDetails(v.Value));
                }
            }

            return result;
        }

        private List<string> GetFormattedDetails(IList<PlaceInfo> places)
        {
            PlaceReportInfo placeReportInfo = null;
            var result = new List<PlaceReportInfo>();
            foreach(var place in places.OrderBy(r => r.FileName).ThenBy(r => r.LineNumber))
            {
                if (placeReportInfo == null || place.FileName != placeReportInfo.FileName)
                {
                    result.Add(placeReportInfo = new PlaceReportInfo());
                }
                placeReportInfo.FileName = place.FileName;
                placeReportInfo.Count++;
                if (!string.IsNullOrEmpty(placeReportInfo.LineNumbers))
                    placeReportInfo.LineNumbers += ", ";
                placeReportInfo.LineNumbers += place.LineNumber;
            }
            return result.OrderByDescending(r => r.Count).ThenBy(r => r.FileName)
                .Select(r => r.ToString()).ToList();
        }
    }
}
