using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using BusinessLayer.CodeClasses;
using NLog;

[assembly: InternalsVisibleTo("UnitTests")]
namespace BusinessLayer.Managers
{
    public class DbRepositoryManager : IManager
    {
        #region Properties and Variables

        private static Logger _logger = LogManager.GetLogger("DbRepositoryManager");

        private string _sourceFolder;

        private string DbRepositoryFolder
        {
            get { return Path.Combine(_sourceFolder, "IFS.DbRepository"); }
        }

        #endregion

        #region Constructors and Initializers

        public void Init(string sourceFolder)
        {
            _sourceFolder = sourceFolder;
        }

        #endregion

        #region Auxilliary methods

        internal static List<string> FilterClasses(IEnumerable<string> fileList)
        {
            List<string> result =
                     (from fileName in fileList
                      where Regex.IsMatch(fileName, @"\w+DbRepository\.cs\b")
                      select Path.GetFileNameWithoutExtension(fileName)).ToList();
            result.Sort();
            return result;
        }

        internal string GetInterfaceFileName(string dbRepositoryName)
        {
            return Path.Combine(_sourceFolder, "IFS.Interfaces", "DbRepository", "I" + dbRepositoryName + ".cs");
        }

        internal string GetDbRepositoryFileName(string dbRepositoryName)
        {
            return Path.Combine(_sourceFolder, "IFS.DbRepository", dbRepositoryName + ".cs");
        }

        internal string GetMockDbRepositoryFileName(string dbRepositoryName)
        {
            return Path.Combine(_sourceFolder, "IFS.NUnitTests", "DbRepository", "Mock" + dbRepositoryName + ".cs");
        }

        private string GetInterfacesProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.Interfaces", "IFS.Interfaces.csproj");
        }

        private string GetDbRepositoryProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.DbRepository", "IFS.DbRepository.csproj");
        }

        private string GetUnitTestsProjectFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.NUnitTests", "IFS.NUnitTests.csproj");
        }

        private string GetCommonSpringConfigFileName()
        {
            return Path.Combine(_sourceFolder, "HedgeFrontier", "Config", "CommonSpringConfig.xml");
        }

        private string GetUnitTestConfigFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.NUnitTests", "Data", "TestData.xml");
        }

        private string GetDbRepositoryFactoryFileName()
        {
            return Path.Combine(_sourceFolder, "IFS.Interfaces", "Factories", "DbRepositoryFactory.cs");
        }

        #endregion

        #region Main Methods

        public List<string> GetClassList()
        {
            if (!Directory.Exists(DbRepositoryFolder)) return null;
            string[] fileList = Directory.GetFiles(DbRepositoryFolder);
            return FilterClasses(fileList);
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

        private void AddMethod(string dbRepositoryName, string methodSignature)
        {
            IMethodData method = new MethodParser(methodSignature);
            if (!method.IsValid)
            {
                _logger.Error("Method is not valid: " + methodSignature);
                return;
            }
            _logger.Info("Adding method " + method.MethodName + " to " + dbRepositoryName);
            _logger.Info("... interface");
            CodeHelper.AddMethodToClass(GetInterfaceFileName(dbRepositoryName), CodeHelper.GetInterfaceSignatureForMethod(method));
            _logger.Info("... DbRepository");
            CodeHelper.AddMethodToClass(GetDbRepositoryFileName(dbRepositoryName), CodeHelper.GetEmptyMethod(method));
            _logger.Info("... MockDbRepository");
            CodeHelper.AddMethodToClass(GetMockDbRepositoryFileName(dbRepositoryName), CodeHelper.GetMockMethod(method, dbRepositoryName));
            _logger.Info("completed.");
        }

        private delegate void DbRepositoryMethod(string dbRepositoryName);

        private void SafeLaunch(DbRepositoryMethod method, string dbRepositoryName)
        {
            try
            {
                method(dbRepositoryName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
            }
        }

        private void CreateInterface(string dbRepositoryName)
        {
            _logger.Info("... adding interface");
            string interfaceName = GetInterfaceFileName(dbRepositoryName);
            if (File.Exists(interfaceName))
                throw new Exception("File " + interfaceName + " already exists");
            File.WriteAllLines(interfaceName, CodeHelper.GetDbRepositoryInterface(dbRepositoryName));
            CodeHelper.IncludeInProject(GetInterfacesProjectFileName(), @"DbRepository\I" + dbRepositoryName + ".cs");
        }

        private void CreateDbRepository(string dbRepositoryName)
        {
            _logger.Info("... adding DbRepository");
            string dbRepositoryFileName = GetDbRepositoryFileName(dbRepositoryName);
            if (File.Exists(dbRepositoryFileName))
                throw new Exception("File " + dbRepositoryFileName + " already exists");
            File.WriteAllLines(dbRepositoryFileName, CodeHelper.GetDbRepository(dbRepositoryName));
            CodeHelper.IncludeInProject(GetDbRepositoryProjectFileName(), dbRepositoryName + ".cs");
        }

        private void CreateMockDbRepository(string dbRepositoryName)
        {
            _logger.Info("... adding MockDbRepository");
            string mockDbRepositoryFileName = GetMockDbRepositoryFileName(dbRepositoryName);
            if (File.Exists(mockDbRepositoryFileName))
                throw new Exception("File " + mockDbRepositoryFileName + " already exists");
            File.WriteAllLines(mockDbRepositoryFileName, CodeHelper.GetMockDbRepository(dbRepositoryName));
            CodeHelper.IncludeInProject(GetUnitTestsProjectFileName(), @"DbRepository\Mock" + dbRepositoryName + ".cs");
        }

        private void ChangeSpringConfigFiles(string dbRepositoryName)
        {
            _logger.Info("... changing Spring config files");
            CodeHelper.IncludeLineInSpringConfig(GetCommonSpringConfigFileName(), "type=\"IFS.DbRepository.",
                CodeHelper.GetSpringConfigLine(dbRepositoryName, "IFS.DbRepository", dbRepositoryName, false));

            CodeHelper.IncludeLineInSpringConfig(GetUnitTestConfigFileName(), "type=\"IFS.NUnitTests.DbRepository.",
                CodeHelper.GetSpringConfigLine(dbRepositoryName, "IFS.NUnitTests", "DbRepository.Mock" + dbRepositoryName, false));
        }

        private void AddFactoryMethod(string dbRepositoryName)
        {
            _logger.Info("... adding factory method");
            CodeHelper.AddMethodToClass(GetDbRepositoryFactoryFileName(), CodeHelper.GetDbRepositoryFactoryMethod(dbRepositoryName));
        }

        public void AddClass(string serviceName)
        {
            if (!serviceName.EndsWith("DbRepository", false, CultureInfo.CurrentCulture))
            {
                _logger.Error(serviceName + " does not end with DbRepository");
                return;
            }

            _logger.Info("Adding " + serviceName + "...");
            SafeLaunch(CreateInterface, serviceName);
            SafeLaunch(CreateDbRepository, serviceName);
            SafeLaunch(CreateMockDbRepository, serviceName);
            SafeLaunch(AddFactoryMethod, serviceName);
            SafeLaunch(ChangeSpringConfigFiles, serviceName);
            _logger.Info("completed.");
        }

        #endregion
    }
}
