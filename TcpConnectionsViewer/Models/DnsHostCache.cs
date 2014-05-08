using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace TcpConnectionsViewer.Models
{
    internal class DnsHostCache
    {
        private static readonly Lazy<DnsHostCache> lazy = new Lazy<DnsHostCache>(() => new DnsHostCache());

        public static DnsHostCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private ConcurrentDictionary<IPAddress, string> DnsCache = new ConcurrentDictionary<IPAddress, string>();
        private ConcurrentDictionary<IPAddress, string> InProgressCache = new ConcurrentDictionary<IPAddress, string>();

        public string Resolve(IPAddress ipAddress)
        {
            while (InProgressCache.ContainsKey(ipAddress))
            {
                //Debug.WriteLine("DNS: already being resolved {0}", ipAddress);
                Thread.Sleep(50);
            }

            string existing;
            if (DnsCache.TryGetValue(ipAddress, out existing))
            {
                //Debug.WriteLine("DNS: found in cache {0}", ipAddress);
                return existing;
            }

            InProgressCache.TryAdd(ipAddress, null);

            var result = PerformDnsLookup(ipAddress);

            // If result.AddressList doesnt contain the IP originally looked up then a new IPAddress entry must manually be added to the cache
            if (!result.AddressList.Any(x => x.Equals(ipAddress)) && !DnsCache.TryGetValue(ipAddress, out existing))
                DnsCache.TryAdd(ipAddress, result.HostName);

            for (int i = 0; i < result.AddressList.Length; i++)
            {
                DnsCache.TryAdd(result.AddressList[i], result.HostName);
            }

            string bleh;
            InProgressCache.TryRemove(ipAddress, out bleh);

            return result.HostName;
        }

        private static IPHostEntry PerformDnsLookup(IPAddress ipAddress)
        {
            try
            {
                if (ipAddress.ToString() == "0.0.0.0")
                    return new IPHostEntry() { AddressList = new IPAddress[] { ipAddress }, HostName = "localhost" };

                //Debug.WriteLine("DNS: resolving {0} via thread {1}", ipAddress.ToString(), Thread.CurrentThread.ManagedThreadId);
                return Dns.GetHostEntry(ipAddress);
            }
            catch (ArgumentException)
            {
                // Return bogus entry so another dns call is not attempted in the future
                return new IPHostEntry() { AddressList = new IPAddress[] { ipAddress } };
            }
            catch (SocketException)
            {
                // Return bogus entry so another dns call is not attempted in the future
                return new IPHostEntry() { AddressList = new IPAddress[] { ipAddress } };
            }
        }

    }
}