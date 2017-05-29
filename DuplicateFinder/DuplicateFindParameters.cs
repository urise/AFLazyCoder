using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AFDuplicateFinder.LanguageConfigurations;

namespace AFDuplicateFinder
{
    public class DuplicateFindParameters
    {
        public int NumberOfLinesLowerLimit { get; set; }
        public ILanguage Language { get; set; }

        public void SetDefault()
        {
            NumberOfLinesLowerLimit = 2;
            Language = new DefaultLanguage();
        }

        public DuplicateFindParameters()
        {
            SetDefault();
        }

        public DuplicateFindParameters(int numberOfLinesLowerLimit, ILanguage language)
        {
            NumberOfLinesLowerLimit = numberOfLinesLowerLimit;
            Language = language;
        }
    }
}
