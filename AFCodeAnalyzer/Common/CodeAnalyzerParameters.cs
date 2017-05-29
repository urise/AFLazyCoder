using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using BusinessLayer.CodeAnalyzer.Searching;
using BusinessLayer.Common;
using BusinessLayer.Helpers;
using BusinessLayer.InfoClasses.Common;

namespace AFCodeAnalyzer.Common
{
    public enum CodeAnalyzerCommand
    {
        SearchLiterals,
        SearchPattern,
        SearchDuplicates
    }

    [Serializable]
    [XmlRoot("root")]
    public class CodeAnalyzerParameters
    {
        [XmlElement("command")]
        public string CommandString { get; set; }

        [XmlElement("pattern")]
        public string Pattern { get; set; }

        [XmlElement("directory")]
        public string Directory { get; set; }

        [XmlArray("include")]
        public string[] IncludePatterns { get; set; }

        [XmlArray("exclude")]
        public string[] ExcludePatterns { get; set; }

        [XmlArray("reports")]
        public SearchReportParameters[] Reports { get; set; }

        public CodeAnalyzerCommand Command
        {
            get
            {
                try
                {
                    return (CodeAnalyzerCommand) Enum.Parse(typeof (CodeAnalyzerCommand), CommandString);
                }
                catch
                {
                    throw new CustomException(CommandString + " is wrong command value");
                }
            }
        }

        private void SetDirectory(IList<string> args)
        {
            if (args.Count != 1)
                throw new CustomException("One and only one directory is allowed");
            Directory = args[0];
        }

        private void SetCommand(IList<string> args)
        {
            if (args.Count != 1)
                throw new CustomException("One and only one search option is allowed");
            CommandString = args[0];
        }

        public void Init(ProgramParameters parameters)
        {
            foreach(var parameter in parameters)
            {
                switch(parameter.Command)
                {
                    case "d":
                        SetDirectory(parameter.Arguments);
                        break;
                    case "s":
                        SetCommand(parameter.Arguments);
                        break;
                }
            }
        }

        public bool SearchFilter(string fileName)
        {
            foreach (var pattern in ExcludePatterns)
            {
                if (TextHelper.Wildcardmatch(fileName, pattern))
                    return false;
            }
            foreach (var pattern in IncludePatterns)
            {
                if (TextHelper.Wildcardmatch(fileName, pattern))
                    return true;
            }
            return false;
        }

        public Func<string, List<string>> GetExtractFunction()
        {
            switch (Command)
            {
                case CodeAnalyzerCommand.SearchPattern:
                    return s => TextHelper.ExtractByPattern(s, Pattern);
                default:
                    return s => TextHelper.ExtractStringLiterals(s);
            }

        }
    }
}
