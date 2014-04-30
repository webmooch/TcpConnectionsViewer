using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace Tests.Converters
{
    [TestClass]
    public class DnsHostCacheTests
    {
        // TODO: Tests that differentiate between hostnames pulled back from cache vs. on-demand resolution

        [TestMethod]
        public void DnsHostCache_ResolveIp1Test()
        {
            var response = DnsHostCache.Instance.Resolve(IPAddress.Parse("203.134.64.66"));
            Assert.IsTrue(response == "nc1.syd.iprimus.net.au");
        }

        [TestMethod]
        public void DnsHostCache_ResolveIp2Test()
        {
            var response = DnsHostCache.Instance.Resolve(IPAddress.Parse("203.134.65.66"));
            Assert.IsTrue(response == "nc2.syd.iprimus.net.au");
        }

        [TestMethod]
        public void DnsHostCache_ResolveIp3Test()
        {
            var response = DnsHostCache.Instance.Resolve(IPAddress.Parse("8.8.8.8"));
            Assert.IsTrue(response == "google-public-dns-a.google.com");
        }
    }
}
