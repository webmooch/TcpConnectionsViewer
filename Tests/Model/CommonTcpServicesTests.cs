using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace ModelTests
{
    [TestClass]
    public class CommonTcpServicesTests
    {
        [TestMethod]
        public void CommonTcpServices_Port0ReservedTest()
        {
            var response = CommonTcpServices.Instance.Lookup(0);
            Assert.IsTrue(response.Port == 0);
            Assert.IsTrue(response.Name == "reserved");
            Assert.IsTrue(response.Description == "Reserved");
        }

        [TestMethod]
        public void CommonTcpServices_Port1TcpMuxTest()
        {
            var response = CommonTcpServices.Instance.Lookup(1);
            Assert.IsTrue(response.Port == 1);
            Assert.IsTrue(response.Name == "tcpmux");
            Assert.IsTrue(response.Description == "TCP Port Service Multiplexer");
        }

        [TestMethod]
        public void CommonTcpServices_Port49151ReservedTest()
        {
            var response = CommonTcpServices.Instance.Lookup(49151);
            Assert.IsTrue(response.Port == 49151);
            Assert.IsTrue(response.Name == "iana reserved");
            Assert.IsTrue(response.Description == "IANA Reserved");
        }

        [TestMethod]
        public void CommonTcpServices_Port49150UnassignedTest()
        {
            var response = CommonTcpServices.Instance.Lookup(49150);
            Assert.IsTrue(response.Port == 49150);
            Assert.IsTrue(response.Name == "unassigned");
            Assert.IsTrue(response.Description == "Unassigned");
        }

        [TestMethod]
        public void CommonTcpServices_Port49152UndefinedTest()
        {
            var response = CommonTcpServices.Instance.Lookup(49152);
            Assert.IsTrue(response.Port == 49152);
            Assert.IsTrue(response.Name == null);
            Assert.IsTrue(response.Description == null);
        }

    }
}
