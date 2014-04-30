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
    public class IpTests
    {
        [TestMethod]
        public void Ip_IsLocalIpTest()
        {
            var localAddrs = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(x => x).ToList();
            localAddrs.Add(IPAddress.Parse("127.0.0.1"));
            localAddrs.Add(IPAddress.Parse("0.0.0.0"));

            for (int i = 0; i < localAddrs.Count; i++)
            {
                Assert.IsTrue(Ip.Instance.IsLocal(localAddrs[i]));
            }
        }
    }
}
