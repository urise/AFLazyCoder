using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder.LanguageConfigurations
{
    public class DefaultLanguage: ILanguage
    {
        public bool CannotStart(string line)
        {
            return FullIgnore(line);
        }

        public bool CannotEnd(string line)
        {
            return FullIgnore(line);
        }

        public bool DontCount(string line)
        {
            return FullIgnore(line);
        }

        public bool FullIgnore(string line)
        {
            return string.IsNullOrEmpty(line);
        }

        public IEnumerable<string> PreliminaryProcess(IEnumerable<string> lines)
        {
            return lines;
        }
    }
}
