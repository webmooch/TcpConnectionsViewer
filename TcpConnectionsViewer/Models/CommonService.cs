using System.Runtime.Serialization;

namespace TcpConnectionsViewer.Models
{
    [DataContract]
    internal class CommonService
    {
        [DataMember(Name = "n")]
        public string Name { get; set; }

        [DataMember(Name = "p")]
        public ushort Port { get; set; }

        [DataMember(Name = "d")]
        public string Description { get; set; }
    }
}