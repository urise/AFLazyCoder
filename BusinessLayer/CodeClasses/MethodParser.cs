using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace BusinessLayer.CodeClasses
{
    public class MethodParser : IMethodData
    {
        public string FullSignature { get; private set; }
        public string ReturnType { get; private set; }
        public string MethodName { get; private set; }
        public string Parameters { get; private set; }
        public bool IsValid { get; private set; }
        public string ParametersWithoutTypes { get; private set; }

        private string GetParametersWithoutTypes(string str)
        {
            var result = new StringBuilder();
            foreach (string par in str.Split(',').Select(r => r.Trim()))
            {
                var match = Regex.Match(par, @"\S+\s+(\S+)");
                if (match.Success)
                {
                    if (result.Length > 0) result.Append(", ");
                    result.Append(match.Groups[1]);
                }
            }
            return result.ToString();
        }

        public MethodParser(string methodSignature)
        {
            const string pattern = @"^(\S+)\s+(\S+)[(](.*)[)]$";
            var regex = new Regex(pattern);
            var match = regex.Match(methodSignature.Trim());
            IsValid = match.Success;
            if (!IsValid) return;

            FullSignature = methodSignature;
            ReturnType = match.Groups[1].ToString();
            MethodName = match.Groups[2].ToString();
            Parameters = match.Groups[3].ToString();
            ParametersWithoutTypes = GetParametersWithoutTypes(Parameters);
        }
    }
}
