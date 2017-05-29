using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BusinessLayer.Helpers;
using BusinessLayer.InfoClasses;
using NUnit.Framework;

namespace UnitTests.InfoClasses
{
    [TestFixture]
    public class CloudServiceInfoTests
    {
        [Test]
        public void SimpleTest()
        {
            var cloudServiceInfo = new CloudServiceInfo("NewService");
            Assert.AreEqual(string.Empty, cloudServiceInfo.SubFolderName);
            Assert.AreEqual(string.Empty, cloudServiceInfo.NamespaceName);
            Assert.AreEqual("NewService", cloudServiceInfo.ServiceName);
            Assert.AreEqual(AppConfiguration.CloudServiceMainBaseClass, cloudServiceInfo.BaseClassName);
        }

        [Test]
        public void SubfolderTest()
        {
            var cloudServiceInfo = new CloudServiceInfo(@"Report\NewService");
            Assert.AreEqual(@"Report\", cloudServiceInfo.SubFolderName);
            Assert.AreEqual(".Report", cloudServiceInfo.NamespaceName);
            Assert.AreEqual("NewService", cloudServiceInfo.ServiceName);
            Assert.AreEqual(AppConfiguration.CloudServiceMainBaseClass, cloudServiceInfo.BaseClassName);
        }

        [Test]
        public void DeepSubfolderTest()
        {
            var cloudServiceInfo = new CloudServiceInfo(@"Report\SubReport\SubSubReport\NewService");
            Assert.AreEqual(@"Report\SubReport\SubSubReport\", cloudServiceInfo.SubFolderName);
            Assert.AreEqual(".Report.SubReport.SubSubReport", cloudServiceInfo.NamespaceName);
            Assert.AreEqual("NewService", cloudServiceInfo.ServiceName);
            Assert.AreEqual(AppConfiguration.CloudServiceMainBaseClass, cloudServiceInfo.BaseClassName);
        }

        [Test]
        public void BaseClassTest()
        {
            var cloudServiceInfo = new CloudServiceInfo("NewService:BaseTradeService");
            Assert.AreEqual(string.Empty, cloudServiceInfo.SubFolderName);
            Assert.AreEqual(string.Empty, cloudServiceInfo.NamespaceName);
            Assert.AreEqual("NewService", cloudServiceInfo.ServiceName);
            Assert.AreEqual("BaseTradeService", cloudServiceInfo.BaseClassName);
        }

        [Test]
        public void ComplexTest()
        {
            var cloudServiceInfo = new CloudServiceInfo(@"Report\SubReport\SubSubReport\NewService:BaseTradeService");
            Assert.AreEqual(@"Report\SubReport\SubSubReport\", cloudServiceInfo.SubFolderName);
            Assert.AreEqual(".Report.SubReport.SubSubReport", cloudServiceInfo.NamespaceName);
            Assert.AreEqual("NewService", cloudServiceInfo.ServiceName);
            Assert.AreEqual("BaseTradeService", cloudServiceInfo.BaseClassName);
        }
    }
}
