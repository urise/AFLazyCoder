using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLayer.Helpers;

namespace AFDuplicateFinder.LanguageConfigurations
{
    public class CSharpLanguage : ILanguage
    {
        public bool CannotStart(string line)
        {
            return FullIgnore(line) || line == "{" || line == "}";
        }

        public bool CannotEnd(string line)
        {
            return FullIgnore(line) || line == "{" || line == "}";
        }

        public bool DontCount(string line)
        {
            return FullIgnore(line) || line == "{" || line == "}";
        }

        public bool FullIgnore(string line)
        {
            return (string.IsNullOrEmpty(line) || line.StartsWith("using ")
                || line.StartsWith("#region"));
        }

        public IEnumerable<string> PreliminaryProcess(IEnumerable<string> lines)
        {
            var linesList = lines.ToList();
            var stringLiterals = TextHelper.GatherStringLiteralsAndReplaceToEmpty(linesList);
            ProcessSpaces(linesList);
            RemoveComments(linesList);
            AssembleOperators(linesList);
            ProcessUnnessessaryStrings(linesList);
            TextHelper.PutGatheredLiteralsBack(linesList, stringLiterals);
            return linesList;
        }

        private void ProcessSpaces(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i].Trim().Replace('\t', ' ');
                lines[i] = Regex.Replace(line, " +", " ");
            }
        }

        private void ProcessUnnessessaryStrings(List<string> lines)
        {
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("using ") || lines[i].StartsWith("#region "))
                    lines[i] = "";
            }
        }

        private int IndexOfFirstComment(string line)
        {
            int firstTypeCommentIndex = line.IndexOf("//");
            int secondTypeCommentIndex = line.IndexOf("/*");
            return firstTypeCommentIndex != -1 && (secondTypeCommentIndex == -1 || firstTypeCommentIndex < secondTypeCommentIndex)
                       ? firstTypeCommentIndex
                       : secondTypeCommentIndex;
        }

        private void RemoveComments(List<string> lines)
        {
            bool secondTypeCommentIsOpenedOnPreviousLine = false;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (secondTypeCommentIsOpenedOnPreviousLine)
                {
                    int closeIndex = line.IndexOf("*/");
                    if (closeIndex == -1)
                    {
                        lines[i] = string.Empty;
                        continue;
                    }
                    line = line.Remove(0, closeIndex + 2);
                }
                while (true)
                {
                    int commentIndex = IndexOfFirstComment(line);
                    if (commentIndex == -1) break;
                    if (line[commentIndex + 1] == '/')
                    {
                        line = line.Remove(commentIndex);
                        break;
                    }
                    int closeIndex = line.IndexOf("*/", commentIndex + 2);
                    if (closeIndex == -1)
                    {
                        secondTypeCommentIsOpenedOnPreviousLine = true;
                        line = line.Remove(commentIndex);
                        break;
                    }
                    line = line.Remove(commentIndex, closeIndex - commentIndex + 2);
                }
                lines[i] = line;
            }
        }

        private void Assemble(List<string> lines, int firstIndex, int lastIndex)
        {
            for (int i = firstIndex + 1; i <= lastIndex; i++)
            {
                lines[firstIndex] += " " + lines[i];
                lines[i] = string.Empty;
            }
        }

        private void AssembleOperators(List<string> lines)
        {
            int index = -1;
            for (int i = 0; i < lines.Count; i++)
            {
                var line = lines[i];
                if (string.IsNullOrEmpty(line)) continue;
                var lastchar = line[line.Length - 1];
                if (index == -1)
                {
                    if (lastchar != ';' && lastchar != '{')
                    {
                        index = i;
                    }
                }
                else
                {
                    if (line[0] == '{')
                    {
                        Assemble(lines, index, i - 1);
                        index = i;
                    }
                    else if (line.IndexOf(';') != -1)
                    {
                        Assemble(lines, index, i);
                        index = i + 1;
                    }
                }
            }
        }
    }
}
