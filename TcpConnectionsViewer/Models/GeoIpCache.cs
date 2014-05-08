using System;
using System.Collections.Concurrent;
using System.Net;
using System.Threading;

namespace TcpConnectionsViewer.Models
{
    internal class GeoIpCache
    {
        private static readonly Lazy<GeoIpCache> lazy = new Lazy<GeoIpCache>(() => new GeoIpCache());

        public static GeoIpCache Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private static ConcurrentDictionary<IPAddress, GeoIpEntry> GeoCache = new ConcurrentDictionary<IPAddress, GeoIpEntry>();
        private static ConcurrentDictionary<IPAddress, string> InProgressCache = new ConcurrentDictionary<IPAddress, string>();

        public GeoIpEntry Lookup(IPAddress ipAddress)
        {
            while (InProgressCache.ContainsKey(ipAddress))
            {
                //Debug.WriteLine("GEO: entry already being looked up {0}", ipAddress);
                Thread.Sleep(50);
            }

            GeoIpEntry existing;
            if (GeoCache.TryGetValue(ipAddress, out existing))
            {
                //Debug.WriteLine("GEO: found in cache {0}", ipAddress);
                return existing;
            }
            else
            {
                //Debug.WriteLine("GEO: NOT found in cache {0}", ipAddress);
            }

            InProgressCache.TryAdd(ipAddress, null);

            GeoIpEntry result;

            if (Ip.Instance.IsLocal(ipAddress))
                result = new GeoIpEntry() { Ip = ipAddress.ToString(), Country = "localhost", Isp = "localhost" };
            else // TODO: else if is on local subnet
                result = LookupFromWeb(ipAddress);

            GeoCache.TryAdd(ipAddress, result);

            string bleh;
            InProgressCache.TryRemove(ipAddress, out bleh);

            return result;
        }

        private static GeoIpEntry LookupFromWeb(IPAddress ip)
        {
            try
            {
                using (var client = new WebClient())
                {
                    var response = client.DownloadString(new Uri(string.Format("http://www.telize.com/geoip/{0}", ip)));
                    return response.DeserializeAsJsonObject<GeoIpEntry>();
                }
            }
            catch (Exception ex)
            {
                if (!ex.IsCritical())
                    return new GeoIpEntry() { Ip = ip.ToString() };
                else
                    throw;
            }
        }

    }
}