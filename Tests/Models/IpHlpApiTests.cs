using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace Tests.Converters
{
    [TestClass]
    public class IpHlpApiTests
    {

        [TestMethod]
        public void IpHlpApi_GetTcpTableExTest()
        {
            MakeConnection("http://hotmail.com");
            var remoteIps = GetIps("hotmail.com");
            Assert.IsTrue(AddressIsFoundInTcpTable(remoteIps));
        }

        [TestMethod]
        public void IpHlpApi_CloseRemoteTcpConnectionTest()
        {
            if (!IsElevated())
            {
                Assert.Inconclusive("Must be run as administrator (or elevated) in order to perform this test");
                return;
            }

            MakeConnection("http://hotmail.com");
            var remoteIps = GetIps("hotmail.com");
            Assert.IsTrue(AddressIsFoundInTcpTable(remoteIps));
            foreach (var item in remoteIps)
            {
                IpHlpApi.CloseRemoteTcpConnection(item);
            }
            Assert.IsTrue(!AddressIsFoundInTcpTable(remoteIps));
        }

        // TODO: ResolvePhysicalAddress - not sure how to consistently test 

        private static void MakeConnection(string address)
        {
            new WebClient().DownloadString(address);
        }

        private static IPAddress[] GetIps(string host)
        {
            return Dns.GetHostEntry(host).AddressList;
        }

        private static bool AddressIsFoundInTcpTable(IPAddress[] addresses)
        {
            var tcpTable = IpHlpApi.GetTcpTableEx();
            bool addressFound = false;
            for (int i = 0; i < tcpTable.Length; i++)
            {
                if (addresses.Any(x => x.ToString() == tcpTable[i].RemoteAddress.ToString()))
                {
                    addressFound = true;
                    break;
                }
            }
            return addressFound;
        }

        private static bool IsElevated()
        {
            var principal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            return principal.IsInRole(WindowsBuiltInRole.Administrator);
        }
    }
}
