using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLayer.CodeClasses;
using BusinessLayer.Helpers;
using BusinessLayer.InfoClasses;
using NLog;

namespace BusinessLayer.Managers
{
    public class CloudServiceManager : IManager
    {
        #region Properties and Variables

        private static Logger _logger = LogManager.GetLogger("CloudServiceManager");

        private string _sourceFolder;

        private string CloudServiceFolder
        {
            get { return Path.Combine(_sourceFolder, @"IFS.BusinessLayer\CloudServices"); }
        }

        private List<string> _cloudServiceBaseClassList;
        private List<string> CloudServiceBaseClassList
        {
            get
            {
                if (_cloudServiceBaseClassList == null)
                {
                    _cloudServiceBaseClassList = string.IsNullOrEmpty(AppConfiguration.CloudServiceBaseClasses)
                                                     ? new List<string>()
                                                     : AppConfiguration.CloudServiceBaseClasses.Split(',').Select(r => r.Trim()).ToList();
                }
                return _cloudServiceBaseClassList;
            }
        }

        #endregion

        #region Constructors and Initializers

        public void Init(string sourceFolder)
        {
            _sourceFolder = sourceFolder;
        }

        #endregion

        #region Auxilliary methods

        private string GetServiceProxyFileName(CloudServiceInfo serviceInfo)
        {
            string folderName = Path.Combine(_sourceFolder, "IFS.CloudProxies", serviceInfo.SubFolderName);
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

            return Path.Combine(folderName, serviceInfo.ServiceName + "Proxy.cs");
        }

        private string GetServiceFileName(CloudServiceInfo serviceInfo)
        {
            string folderName = Path.Combine(_sourceFolder, "IFS.BusinessLayer", "CloudServices", serviceInfo.SubFolderName);
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

            return Path.Combine(folderName, serviceInfo.ServiceName + ".cs");
        }

        private string GetInterfaceFileName(CloudServiceInfo serviceInfo)
        {
            string folderName = Path.Combine(_sourceFolder, "IFS.Interfaces", "CloudContracts", serviceInfo.SubFolderName);
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

            return Path.Combine(folderName, "I" + serviceInfo.ServiceName + ".cs");
        }

        private string GetCloudProxiesProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.CloudProxies", "IFS.CloudProxies.csproj");
        }

        private string GetInterfacesProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.Interfaces", "IFS.Interfaces.csproj");
        }

        private string GetAppistryProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.BusinessLayer.Appistry", "IFS.BusinessLayer.Appistry.csproj");
        }

        private string GetBusinessLayerProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.BusinessLayer", "IFS.BusinessLayer.csproj");
        }

        private string GetCloudSpringConfigFileName()
        {
            return Path.Combine(_sourceFolder, "HedgeFrontier", "Config", "CloudSpringConfig.xml");
        }

        private string GetCloudSpringConfigDevFileName()
        {
            return Path.Combine(_sourceFolder, "HedgeFrontier", "Config", "CloudSpringConfigDev.xml");
        }

        private string GetComponentsFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.BusinessLayer.Appistry", "Appistry", "Components", "Components.xml");
        }

        private string GetServiceNamesFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.BusinessLayer", "CloudServices", "ServiceNames.cs");
        }

        #endregion

        #region Main Methods

        private static List<string> FilterClasses(IEnumerable<string> fileList)
        {
            List<string> result =
                     (from fileName in fileList
                      where Regex.IsMatch(fileName, @"\w+Service\.cs\b")
                      select Path.GetFileNameWithoutExtension(fileName)).ToList();
            result.Sort();
            return result;
        }

        public List<string> GetClassList()
        {
            if (!Directory.Exists(CloudServiceFolder)) return null;
            return GetClassList(CloudServiceFolder);
        }

        private List<string> GetClassList(string folder)
        {
            int len = CloudServiceFolder.Length;
            string shortFolder = string.Empty;
            if (folder.Length > len)
            {
                shortFolder = folder.Substring(len + 1);
            }
            var result = new List<string>();
            string[] subFolderList = Directory.GetDirectories(folder);
            foreach(var subFolder in subFolderList)
            {
                result.AddRange(GetClassList(subFolder));
            }
            
            string[] fileList = Directory.GetFiles(folder);
            foreach(var fileName in fileList)
            {
                var content = File.ReadAllLines(fileName);
                string classLine = content.FirstOrDefault(r => r.Trim().Contains(" class "));
                if (string.IsNullOrEmpty(classLine)) continue;

                foreach(var baseClass in CloudServiceBaseClassList)
                {
                    var match = Regex.Match(classLine, @"public\s+class\s+(\w+)\s*[:]?.*\W?" + baseClass + @"(<\w+>)?\W?");
                    if (match.Success && !CloudServiceBaseClassList.Contains(match.Groups[1].ToString()))
                    {
                        result.Add(Path.Combine(shortFolder, match.Groups[1].ToString()));    
                    }
                }
            }
            return result;
        }

        public void AddMethods(string className, IEnumerable<string> methods)
        {
            foreach (string method in methods)
            {
                if (!string.IsNullOrEmpty(method))
                {
                    AddMethod(className, method);
                }
            }
        }

        private int AddProcessFlow(CloudServiceInfo serviceInfo, string methodName)
        {
            string folderName = Path.Combine(_sourceFolder, "IFS.BusinessLayer.Appistry", "Appistry", "ProcessFlows",
                                           serviceInfo.SubFolderName, serviceInfo.ServiceName);
            if (!Directory.Exists(folderName)) Directory.CreateDirectory(folderName);

            string fileName = Path.Combine(folderName, methodName + ".xml");
            List<string> content = CodeHelper.GetCodeFromPattern("ProcessFlow").Select(
                r => r.Replace(@"%SERVICE_NAME%", serviceInfo.ServiceName)
                .Replace(@"%METHOD_NAME%", methodName)).ToList();
            File.WriteAllLines(fileName, content);

            CodeHelper.IncludeInProject(GetAppistryProjectFileName(), 
                Path.Combine("Appistry", "ProcessFlows", serviceInfo.SubFolderName, serviceInfo.ServiceName, 
                serviceInfo.ServiceName + "_" + methodName + ".xml"), false);
            return 0;
        }

        private void AddMethod(string serviceName, string methodSignature)
        {
            IMethodData method = new MethodParser(methodSignature);
            if (!method.IsValid)
            {
                _logger.Error("Method is not valid: " + methodSignature);
                return;
            }
            var serviceInfo = new CloudServiceInfo(serviceName);
            _logger.Info("Adding method " + method.MethodName + " to " + serviceName);
            _logger.Info("... interface");
            SafeLaunch(CodeHelper.AddMethodToClass, GetInterfaceFileName(serviceInfo), CodeHelper.GetInterfaceSignatureForMethod(method));
            _logger.Info("... service");
            SafeLaunch(CodeHelper.AddMethodToClass, GetServiceFileName(serviceInfo), CodeHelper.GetEmptyMethod(method));
            _logger.Info("... service proxy");
            SafeLaunch(CodeHelper.AddMethodToClass, GetServiceProxyFileName(serviceInfo), CodeHelper.GetProxyMethod(method, serviceInfo));
            _logger.Info("... process flow");
            SafeLaunch(r => r.AddProcessFlow(serviceInfo, method.MethodName));
            _logger.Info("completed.");
        }

        private delegate void CloudServiceMethod(CloudServiceInfo serviceInfo);

        private void SafeLaunch(CloudServiceMethod method, CloudServiceInfo serviceInfo)
        {
            try
            {
                method(serviceInfo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private delegate void SaveToFileMethod(string fileName, IEnumerable<string> content);

        private void SafeLaunch(SaveToFileMethod method, string fileName, IEnumerable<string> content)
        {
            try
            {
                method(fileName, content);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void SafeLaunch(Func<CloudServiceManager, int> func)
        {
            try
            {
                func(this);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void CreateService(CloudServiceInfo serviceInfo)
        {
            _logger.Info("... adding Service");
            string fileName = GetServiceFileName(serviceInfo);
            if (File.Exists(fileName)) throw new Exception("File " + fileName + " already exists");

            File.WriteAllLines(fileName, CodeHelper.GetServiceCodeFromPattern("CloudService", serviceInfo));
            CodeHelper.IncludeInProject(GetBusinessLayerProjectFileName(), @"CloudServices\" + serviceInfo.SubFolderName + serviceInfo.ServiceName + ".cs");
        }
        
        private void CreateServiceProxy(CloudServiceInfo serviceInfo)
        {
            _logger.Info("... adding ServiceProxy");
            string fileName = GetServiceProxyFileName(serviceInfo);
            if (File.Exists(fileName))
                throw new Exception("File " + fileName + " already exists");
            File.WriteAllLines(fileName, CodeHelper.GetServiceCodeFromPattern("CloudServiceProxy", serviceInfo));
            CodeHelper.IncludeInProject(GetCloudProxiesProjectFileName(), serviceInfo.SubFolderName + serviceInfo.ServiceName + "Proxy.cs");
        }

        private void CreateInterface(CloudServiceInfo serviceInfo)
        {
            _logger.Info("... adding interface");
            string fileName = GetInterfaceFileName(serviceInfo);
            if (File.Exists(fileName))
                throw new Exception("File " + fileName + " already exists");
            File.WriteAllLines(fileName, CodeHelper.GetServiceCodeFromPattern("ICloudService", serviceInfo));
            CodeHelper.IncludeInProject(GetInterfacesProjectFileName(), @"CloudContracts\" + serviceInfo.SubFolderName + "I" + serviceInfo.ServiceName + ".cs");
        }

        private void ChangeConfigFiles(CloudServiceInfo serviceInfo)
        {
            _logger.Info("... changing config files");
            string serviceId = serviceInfo.ServiceName.FirstToLower();
            CodeHelper.IncludeLineInSpringConfig(GetCloudSpringConfigFileName(),
                "</objects>",
                "  <object id=\"" + serviceId + "\" type=\"IFS.CloudProxies" + serviceInfo.NamespaceName + "." + serviceInfo.ServiceName + "Proxy, IFS.CloudProxies\" lazy-init=\"true\" />", 0);
            CodeHelper.IncludeLineInSpringConfig(GetCloudSpringConfigDevFileName(),
                "</objects>",
                "  <object id=\"" + serviceId + "\" type=\"IFS.BusinessLayer.CloudServices" + serviceInfo.NamespaceName + "." + serviceInfo.ServiceName + ", IFS.BusinessLayer\" lazy-init=\"true\" />", 0);
            
            var sb = new StringBuilder();
            sb.AppendLine("  <component name=\"" + serviceInfo.ServiceName + "\">");
            sb.AppendLine("    <class name=\"IFS.BusinessLayer.CloudServices" + serviceInfo.NamespaceName + "." + serviceInfo.ServiceName + ", IFS.BusinessLayer\"/>");
            sb.Append("  </component>");
            CodeHelper.IncludeLineInSpringConfig(GetComponentsFileName(),
                "</dotnet-components>",
                sb.ToString(), 0);

            CodeHelper.IncludeLineInSpringConfig(GetServiceNamesFileName(),
                "public",
                "public static string " + serviceInfo.ServiceName.ConvertToConstName() + "_SPRING_NAME = \"" + serviceInfo.ServiceName.FirstToLower() + "\";");
        }

        public void AddClass(string serviceName)
        {
            var serviceInfo = new CloudServiceInfo(serviceName);
            if (string.IsNullOrEmpty(serviceInfo.ServiceName))
            {
                _logger.Error(serviceName + " : wrong format");
            }
            if (!serviceName.EndsWith("Service", false, CultureInfo.CurrentCulture))
            {
                _logger.Error(serviceName + " does not end with Service");
                return;
            }

            _logger.Info("Adding " + serviceName + "...");
            SafeLaunch(CreateService, serviceInfo);
            SafeLaunch(CreateServiceProxy, serviceInfo);
            SafeLaunch(CreateInterface, serviceInfo);
            SafeLaunch(ChangeConfigFiles, serviceInfo);
            _logger.Info("completed.");
        }

        #endregion
    }
}
