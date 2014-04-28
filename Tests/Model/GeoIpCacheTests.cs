using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace ModelTests
{
    [TestClass]
    public class GeoIpCacheTests
    {

        [TestMethod]
        public void GeoIpCache_GoogleDnsGeoIpLookupTest()
        {
            var result = GeoIpCache.Instance.Lookup(IPAddress.Parse("8.8.8.8"));
            Assert.IsTrue(result.Ip == "8.8.8.8");
            Assert.IsTrue(result.Country == "United States");
            Assert.IsTrue(result.Isp == "Google Inc.");
            Assert.IsTrue(result.Longitude == "-97");
            Assert.IsTrue(result.Latitude == "38");
            Assert.IsTrue(result.Country_Code == "US");
            Assert.IsTrue(result.Country_Code3 == "USA");
        }

        [TestMethod]
        public void GeoIpCache_LocalhostGeoIpLookupTest()
        {
            var result = GeoIpCache.Instance.Lookup(IPAddress.Parse("127.0.0.1"));
            Assert.IsTrue(result.Isp == "localhost");
            Assert.IsTrue(result.Country == "localhost");
            Assert.IsTrue(result.Ip == "127.0.0.1");
        }
    }
}