using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace BusinessLayer.Helpers
{
    public static class AppConfiguration
    {
        public static string CloudServiceBaseClasses
        {
            get { return ConfigurationManager.AppSettings["CloudServiceBaseClasses"]; }
        }

        public static string AlphaFrontierSourceFolder
        {
            get { return ConfigurationManager.AppSettings["AlphaFrontierSourceFolder"]; }
        }

        public static string CloudServiceMainBaseClass
        {
            get { return ConfigurationManager.AppSettings["CloudServiceMainBaseClass"]; }
        }
    }
}
