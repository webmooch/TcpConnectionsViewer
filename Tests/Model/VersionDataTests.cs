using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace ModelTests
{
    [TestClass]
    public class VersionDataTests
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessMonitor_GetVersionDataInvalidStaticVersionLinkTest()
        {
            VersionData.GetVersionData("http://google.com");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void ProcessMonitor_GetVersionDataInvalidDynamicVersionLinkTest()
        {
            var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".temp";
            File.WriteAllText(tempFile, "!@#$%^&");
            VersionData.GetVersionData(tempFile);
            File.Delete(tempFile);
        }

        [TestMethod]
        [ExpectedException(typeof(SerializationException))]
        public void ProcessMonitor_GetVersionDataInvalidJsonTest()
        {
            var tempFile = Path.GetTempPath() + Guid.NewGuid().ToString() + ".temp";
            File.WriteAllText(tempFile, "http://google.com");
            VersionData.GetVersionData(tempFile);
            File.Delete(tempFile);
        }

        [TestMethod]
        public void ProcessMonitor_GetVersionDataIsValidTest()
        {
            var versionData = VersionData.GetVersionData(TcpConnectionsViewer.Properties.Settings.Default.StaticVersionUri);
            Assert.IsTrue(!string.IsNullOrWhiteSpace(versionData.DownloadUri));
            Assert.IsTrue(versionData.IsValid);
        }
    }
}