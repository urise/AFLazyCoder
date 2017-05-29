using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFDuplicateFinder.LanguageConfigurations;
using AFDuplicateFinder.Results;

namespace AFDuplicateFinder
{
    public class DuplicateFinder
    {
        #region Fields and Properties

        private readonly FileLine[] _fileLines;
        private readonly FileLine[] _sortedFileLines;
        private readonly DuplicateFindParameters _parameters;

        private int GetLinesCount()
        {
            return _fileLines.Length;
        }

        private FileLine GetFileLine(int n)
        {
            return _fileLines[n];
        }

        private FileLine GetSortedFileLine(int n)
        {
            return _sortedFileLines[n];
        }

        public DuplicateFinder(IEnumerable<FileContent> fileContents, DuplicateFindParameters parameters = null)
        {
            _parameters = parameters ?? new DuplicateFindParameters();

            int n = 0;
            const int limit = 10000000;
            int linesCount = fileContents.Sum(fileContent => fileContent.Lines.Count());
            if (linesCount > limit) linesCount = limit;
            _fileLines = new FileLine[linesCount];

            foreach(var fileContent in fileContents)
            {
                int lineNumber = 1;
                foreach(var line in fileContent.Lines)
                {
                    var fileLine = new FileLine(fileContent.FileName, line.Trim(), lineNumber, n);
                    _fileLines[n] = fileLine;
                    lineNumber++;
                    n++;
                    if (n >= limit) break;
                }
                if (n >= limit) break;
            }
            _sortedFileLines = _fileLines.OrderBy(r => r.Line).ThenBy(r => r.FileName).ThenBy(r => r.LineNumber).ToArray();
        }

        #endregion

        #region Methods

        private int GetNextLineIndex(int unsortedIndex, ILanguage language, int direction)
        {
            var fileName = _fileLines[unsortedIndex].FileName;
            for (int i = unsortedIndex + direction; i > 0 && i < _fileLines.Length; i+=direction)
            {
                if (_fileLines[i].FileName != fileName) break;
                if (!string.IsNullOrEmpty(_fileLines[i].Line)) return i;
            }
            return -1;
        }

        private void AddResultsByDirection(DuplicateInfo result, int index1, int index2, 
            ILanguage language, int direction)
        {
            index1 = GetNextLineIndex(index1, language, direction);
            index2 = GetNextLineIndex(index2, language, direction);
            while (index1 != -1 && index2 != -1 && _fileLines[index1].Line == _fileLines[index2].Line)
            {
                result.Add(index1, index2);
                index1 = GetNextLineIndex(index1, language, direction);
                index2 = GetNextLineIndex(index2, language, direction);
            }
        }

        private void RemoveWrongStartLines(DuplicateInfo duplicateInfo, ILanguage language)
        {
            //while (duplicateInfo.Any() && language.CannotStart(_fileLines[duplicateInfo.First().Index1].Line))
            //{
            //    duplicateInfo.RemoveFirst();
            //}
        }

        private void RemoveWrongEndLines(DuplicateInfo duplicateInfo, ILanguage language)
        {
            //while (duplicateInfo.Any() && language.CannotEnd(_fileLines[duplicateInfo.Last().Index1].Line))
            //{
            //    duplicateInfo.RemoveLast();
            //}
        }

        private DuplicateInfo GetEqualLines(int sortedLineNumber1, int sortedLineNumber2, ILanguage language)
        {
            var result = new DuplicateInfo();
            int unsortedLineNumber1 = _sortedFileLines[sortedLineNumber1].IndexInUnsortedArray;
            int unsortedLineNumber2 = _sortedFileLines[sortedLineNumber2].IndexInUnsortedArray;
            
            result.Add(unsortedLineNumber1, unsortedLineNumber2);
            AddResultsByDirection(result, unsortedLineNumber1, unsortedLineNumber2, language, 1);
            AddResultsByDirection(result, unsortedLineNumber1, unsortedLineNumber2, language, -1);
            RemoveWrongStartLines(result, language);
            RemoveWrongEndLines(result, language);
            return result;
        }

        private void SetPartialLinks(IEnumerable<IndexPair> pairs)
        {
            foreach (var pair in pairs)
            {
                _fileLines[pair.Index2].AddPartialLink(pair.Index1);
            }
        }

        private bool IsPartiallyLinked(int sortedLineNumber1, int sortedLineNumber2)
        {
            var unsortedIndex1 = _sortedFileLines[sortedLineNumber1].IndexInUnsortedArray;
            return _sortedFileLines[sortedLineNumber2].IsPartiallyLinked(unsortedIndex1);

        }

        private void AddDuplicatesStartedFromLine(DuplicateResultFull result, int i)
        {
            var fileLine1 = GetSortedFileLine(i);
            for (int j = i + 1; j < GetLinesCount(); j++)
            {
                var fileLine2 = GetSortedFileLine(j);
                if (fileLine1.Line != fileLine2.Line) break;

                if (IsPartiallyLinked(i, j)) continue;

                var duplicateInfo = GetEqualLines(i, j, _parameters.Language);
                if (duplicateInfo.GetLinesCount() < _parameters.NumberOfLinesLowerLimit) continue;
                SetPartialLinks(duplicateInfo);
                var firstPair = duplicateInfo.GetFirstIndexPair();
                result.Add(duplicateInfo.GetLinesCount(), GetFileLine(firstPair.Index1),
                    GetFileLine(firstPair.Index2));
            }
        }

        public DuplicateResultFull FindDuplicates()
        {
            var result = new DuplicateResultFull();

            for (int i = 0; i < GetLinesCount(); i++)
            {
                var currentLine = GetSortedFileLine(i).Line;
                if (_parameters.Language.CannotStart(currentLine)) continue;

                AddDuplicatesStartedFromLine(result, i);
            }

            return result;
        }

        #endregion
    }
}
