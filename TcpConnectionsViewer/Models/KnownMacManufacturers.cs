using System;
using System.Collections.Generic;
using System.Linq;
using TcpConnectionsViewer.Properties;

namespace TcpConnectionsViewer.Models
{
    internal class KnownMacManufacturers
    {
        private static readonly Lazy<KnownMacManufacturers> lazy = new Lazy<KnownMacManufacturers>(() => new KnownMacManufacturers());

        public static KnownMacManufacturers Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private KnownMacManufacturers()
        {
            jsonData = Resources.KnownMacManufacturers.DeserializeAsJsonObject<List<KnownMacManufacturer>>();
        }

        private List<KnownMacManufacturer> jsonData;

        public KnownMacManufacturer Lookup(string macAddr)
        {
            return jsonData.Where(x => macAddr.StartsWith(x.PrefixId)).Select(y => y).FirstOrDefault();
        }
    }
}