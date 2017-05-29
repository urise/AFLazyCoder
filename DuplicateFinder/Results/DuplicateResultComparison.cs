using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Comparison;

namespace AFDuplicateFinder.Results
{
    public class DuplicateResultComparison
    {
        private ListsComparer<DuplicateResult> _listsComparer;

        public int PlusCount { get { return _listsComparer.OnlyInList2.Sum(r => r.LinesCount); } }
        public int MinusCount { get { return _listsComparer.OnlyInList1.Sum(r => r.LinesCount); } }

        public DuplicateResultComparison(DuplicateResultFull resultFull1,  DuplicateResultFull resultFull2)
        {
            _listsComparer = new ListsComparer<DuplicateResult>(resultFull1, resultFull2);
            _listsComparer.CompareUnsorted();
        }

        public string GetHtmlReport()
        {
            var report = new StringBuilder();
            report.AppendLine("<b>NEW DUPLICATES</b><br><br>");
            foreach (var duplicateResult in _listsComparer.OnlyInList2.OrderByDescending(r => r.LinesCount).ThenByDescending(r => r.GetUnitsCount()))
            {
                report.AppendLine(duplicateResult.ToHtml());
            }
            report.AppendLine("<br>");
            report.AppendLine("<b>DISAPPAIRED DUPLICATES</b><br><br>");
            foreach (var duplicateResult in _listsComparer.OnlyInList1.OrderByDescending(r => r.LinesCount).ThenByDescending(r => r.GetUnitsCount()))
            {
                report.AppendLine(duplicateResult.ToHtml());
            }
            report.AppendLine("<br>");
            return report.ToString();
        }
    }
}
