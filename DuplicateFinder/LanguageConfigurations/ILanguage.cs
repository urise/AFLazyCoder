using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AFDuplicateFinder.LanguageConfigurations
{
    public interface ILanguage
    {
        bool CannotStart(string line);
        //bool CannotEnd(string line);
        //bool DontCount(string line);
        //bool FullIgnore(string line);
        IEnumerable<string> PreliminaryProcess(IEnumerable<string> lines);
    }
}
