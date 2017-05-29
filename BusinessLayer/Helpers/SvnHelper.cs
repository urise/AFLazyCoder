using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace BusinessLayer.Helpers
{
    public static class SvnHelper
    {
        public static List<string> RunProcess(string folder, string exeFullName, string arguments)
        {
            var currentDirectory = Directory.GetCurrentDirectory();
            if (!String.IsNullOrEmpty(folder))
                Directory.SetCurrentDirectory(folder);

            var p = new Process();
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.FileName = exeFullName;
            p.StartInfo.Arguments = arguments;
            p.Start();
            string output = p.StandardOutput.ReadToEnd();
            p.WaitForExit();
            Directory.SetCurrentDirectory(currentDirectory);
            return Regex.Split(output, Environment.NewLine).ToList();
        }

        public static int GetCurrentRevision(string projectFolder)
        {
            List<string> output = RunProcess(projectFolder, @"d:\Program Files\SlikSvn\bin\svn.exe", "info");
            string line = (from str in output where str.StartsWith("Revision:") select str).FirstOrDefault();
            if (String.IsNullOrEmpty(line)) return -1;
            return TextHelper.GetIntAfterSemicolon(line);
        }
    }
}
