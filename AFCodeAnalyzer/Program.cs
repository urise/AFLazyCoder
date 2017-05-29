using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using AFCodeAnalyzer.Common;
using AFDuplicateFinder;
using AFDuplicateFinder.LanguageConfigurations;
using AFDuplicateFinder.Results;
using BusinessLayer.CodeAnalyzer;
using BusinessLayer.CodeAnalyzer.Searching;
using BusinessLayer.Common;
using BusinessLayer.Helpers;
using BusinessLayer.InfoClasses.Common;

namespace AFCodeAnalyzer
{
    class Program
    {
        static void TestXml()
        {
            var resultFull = new DuplicateResultFull();
            var result1 = new DuplicateResult(5);
            result1.AddUnit("1.txt", 222);
            result1.AddUnit("2.txt", 333);
            var result2 = new DuplicateResult(10);
            resultFull.Add(result1);
            resultFull.Add(result2);
            var xml = resultFull.ToXml();
            Console.WriteLine(xml);

            var newResultFull = new DuplicateResultFull();
            newResultFull.InitFromXml(xml);
            Console.ReadKey();
        }

        static void Main(string[] args)
        {
            var time1 = DateTime.Now;
            Console.WriteLine("CurrentDirectory: " + Directory.GetCurrentDirectory());

            try
            {
                var rawParameters = new ProgramParameters();
                rawParameters.Init(args);
                var xmlParameter = rawParameters.FirstOrDefault(r => r.Command == "xml");
                if (xmlParameter == null)
                    throw new Exception("-xml parameter should be provided");
                foreach (var argument in xmlParameter.Arguments)
                {
                    if (!File.Exists(argument))
                        throw new Exception("Config file " + argument + "does not exist");
                    var dispatcher = new AnalyzerDispatcher(File.ReadAllText(argument));
                    dispatcher.Execute();
                }

            }
            catch (System.Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var time2 = DateTime.Now;
            Console.WriteLine("Time(sec): " + (time2 - time1));
            //Console.ReadLine();
        }
    }
}
