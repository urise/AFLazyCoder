using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BusinessLayer.InfoClasses.Common
{
    public class ProgramParameters: IEnumerable<ProgramParameter>
    {
        private List<ProgramParameter> _parameters = new List<ProgramParameter>();

        public void Init(string[] programArgs)
        {
            ProgramParameter programParameter = null;
            foreach(var arg in programArgs)
            {
                if (arg.StartsWith("-"))
                {
                    programParameter = new ProgramParameter() {Command = arg.Substring(1)};
                    _parameters.Add(programParameter);
                }
                else
                {
                    if (programParameter == null)
                        throw new Exception("Bad program parameters: command is expected");
                    programParameter.Arguments.Add(arg);
                }
            }
        }

        public IEnumerator<ProgramParameter> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }
    }
}
