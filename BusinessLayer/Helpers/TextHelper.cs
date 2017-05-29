using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLayer.Helpers
{
    public static class TextHelper
    {
        public static string FirstToLower(this string str)
        {
            return string.IsNullOrEmpty(str) ? str : str.Substring(0, 1).ToLower() + str.Substring(1);
        }

        public static string ConvertToConstName(this string str)
        {
            var result = new StringBuilder();
            bool prevIsLower = false;
            foreach (char c in str)
            {
                if (char.IsUpper(c) && prevIsLower) result.Append('_');
                result.Append(char.ToUpper(c));
                prevIsLower = char.IsLower(c);
            }
            return result.ToString();
        }

        public static List<string> ExtractStringLiterals(string str, bool includeApostrophes = false)
        {
            var result = new List<String>();
            if (string.IsNullOrEmpty(str)) return result;
            bool isSlashMode = false;
            char quotaFound = ' ';
            var sb = new StringBuilder();

            foreach (var c in str)
            {
                if (quotaFound == ' ')
                {
                    if (c == '"' || includeApostrophes && c == '\'') quotaFound = c;
                    continue;
                }

                if (c == quotaFound && !isSlashMode)
                {
                    result.Add(sb.ToString());
                    quotaFound = ' ';
                    sb.Clear();
                    continue;
                }

                sb.Append(c);
                isSlashMode = (c == '\\' && !isSlashMode);
            }
            return result;
        }

        public static List<string> ExtractByPattern(string str, string pattern)
        {
            var result = new List<string>();
            foreach(Match m in Regex.Matches(str, pattern))
            {
                result.Add(m.Value);
            }
            return result;
        }

        public static bool Wildcardmatch(string str, string wildcardPattern)
        {
            var regexPattern = "^" + Regex.Escape(wildcardPattern).
                Replace("\\*", ".*").
                Replace("\\?", ".") + "$";
            return Regex.IsMatch(str.ToLower(), regexPattern.ToLower());
        }

        public static string TransformStringLiterals(this string str, char replaceChar = ' ')
        {
            bool insideQuotas = false;
            bool afterSlash = false;

            var result = new StringBuilder(str.Length);
            foreach (char c in str)
            {
                if (c == '"' && !afterSlash)
                {
                    insideQuotas = !insideQuotas;
                    result.Append('"');
                }
                else
                {
                    afterSlash = c == '\\' && insideQuotas && !afterSlash;
                    result.Append(insideQuotas ? replaceChar : c);
                }
            }
            return result.ToString();
        }

        public static List<string> GatherStringLiteralsAndReplaceToEmpty(List<string> lines)
        {
            var result = new List<string>();
            for (int index = 0; index < lines.Count; index++)
            {
                var line = lines[index];
                var literals = ExtractStringLiterals(line, true);
                if (literals.Count == 0) continue;

                foreach (var literal in literals)
                {
                    line = line.Replace('"' + literal + '"', "\"\"").Replace("'" + literal + "'", "''");
                }
                lines[index] = line;
                result.AddRange(literals);
            }
            return result;
        }

        private static int GetEmptyStringIndex(string line, int startIndex)
        {
            int index1 = line.IndexOf("\"\"", startIndex);
            int index2 = line.IndexOf("''", startIndex);
            return index1 == -1 ? index2 : index2 == -1 ? index1 : Math.Min(index1, index2);
        }

        public static void PutGatheredLiteralsBack(List<string> lines, List<string> literals)
        {
            var indexLiterals = 0;
            for (var indexLines = 0; indexLines < lines.Count; indexLines++)
            {
                var line = lines[indexLines];
                int startIndex = 0;
                while (true)
                {
                    var emptyStringIndex = GetEmptyStringIndex(line, startIndex);
                    if (emptyStringIndex == -1) break;
                    line = line.Insert(emptyStringIndex + 1, literals[indexLiterals]);
                    startIndex = emptyStringIndex + literals[indexLiterals].Length + 2;
                    indexLiterals++;
                }
                lines[indexLines] = line;
            }
        }
        public static string GetStringAfterSemicolon(string str)
        {
            int n = str.IndexOf(":");
            if (n == -1) return String.Empty;
            return str.Substring(n + 1).Trim();
        }

        public static int GetIntAfterSemicolon(string str)
        {
            return Convert.ToInt32(GetStringAfterSemicolon(str));
        }
    }
}
