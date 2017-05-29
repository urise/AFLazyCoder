using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFDuplicateFinder.LanguageConfigurations;

namespace UnitTests.AFDuplicateFinder.Mocks
{
    public class MockLanguage: ILanguage
    {
        public bool CannotStart(string line)
        {
            return FullIgnore(line) || line.StartsWith("??");
        }

        public bool CannotEnd(string line)
        {
            return FullIgnore(line) || line.StartsWith("!!");
        }

        public bool DontCount(string line)
        {
            return FullIgnore(line) || line.StartsWith("@@");
        }

        public bool FullIgnore(string line)
        {
            return string.IsNullOrEmpty(line) || line.StartsWith("//");
        }

        public IEnumerable<string> PreliminaryProcess(IEnumerable<string> lines)
        {
            throw new NotImplementedException();
        }
    }
}
