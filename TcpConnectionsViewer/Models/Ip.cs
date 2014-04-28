using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace TcpConnectionsViewer.Models
{
    internal class Ip
    {
        private static readonly Lazy<Ip> lazy = new Lazy<Ip>(() => new Ip());

        public static Ip Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private Ip()
        {
            localIps = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(x => x).ToList();
            localIps.Add(IPAddress.Loopback);
            localIps.Add(IPAddress.IPv6Loopback);
            localIps.Add(IPAddress.Parse("0.0.0.0"));
        }

        private List<IPAddress> localIps;

        public bool IsLocal(IPAddress ip)
        {
            return IsLocal(ip.ToString());
        }

        public bool IsLocal(string ip)
        {
            return localIps.Any(x => string.Equals(x.ToString(), ip, StringComparison.OrdinalIgnoreCase));
        }

    }
}