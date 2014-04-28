using System;
using System.Collections.Generic;
using System.Linq;
using TcpConnectionsViewer.Properties;

namespace TcpConnectionsViewer.Models
{
    internal class CommonTcpServices
    {
        private static readonly Lazy<CommonTcpServices> lazy = new Lazy<CommonTcpServices>(() => new CommonTcpServices());

        public static CommonTcpServices Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private CommonTcpServices()
        {
            jsonData = Resources.CommonServices.DeserializeAsJsonObject<List<CommonService>>();
        }

        private List<CommonService> jsonData;

        public CommonService Lookup(ushort port)
        {
            return jsonData.Where(x => x.Port == port).Select(y => y).DefaultIfEmpty(new CommonService() { Port = port }).FirstOrDefault();
        }

    }
}