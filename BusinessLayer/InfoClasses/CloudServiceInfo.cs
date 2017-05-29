using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLayer.Helpers;

namespace BusinessLayer.InfoClasses
{
    public class CloudServiceInfo
    {
        public string ServiceName { get; set; }
        public string SubFolderName { get; set; }
        public string NamespaceName { get; set; }
        public string BaseClassName { get; set; }
        public string SubFolderNameWithSlash
        {
            get { return string.IsNullOrEmpty(SubFolderName) ? string.Empty : SubFolderName + @"\"; }
        }

        public CloudServiceInfo(string fullServiceName)
        {
            var match = Regex.Match(fullServiceName, @"\s*(.*\\)?(\w+)\s*[:]?\s*(\w+)?");
            if (match.Success)
            {
                SubFolderName = match.Groups[1].ToString();

                ServiceName = match.Groups[2].ToString();
                BaseClassName = match.Groups[3].ToString();
                if (string.IsNullOrEmpty(BaseClassName))
                    BaseClassName = AppConfiguration.CloudServiceMainBaseClass;
                NamespaceName = string.IsNullOrEmpty(SubFolderName)
                                    ? string.Empty
                                    : "." + SubFolderName.Substring(0, SubFolderName.Length - 1).Replace(@"\", ".");
            }
        }
    }
}
