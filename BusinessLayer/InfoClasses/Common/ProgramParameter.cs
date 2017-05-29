using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.InfoClasses.Common
{
    public class ProgramParameter
    {
        public string Command { get; set; }
        public List<string> Arguments { get; private set; }

        public ProgramParameter()
        {
            Arguments = new List<string>();
        }
    }
}
