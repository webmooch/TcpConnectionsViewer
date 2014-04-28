using System.Runtime.Serialization;

namespace TcpConnectionsViewer.Models
{
    [DataContract]
    public class KnownMacManufacturer
    {
        [DataMember(Name = "p")]
        public string PrefixId { get; set; }

        [DataMember(Name = "n")]
        public string Name { get; set; }

        public override string ToString()
        {
            return Name;
        }
    }
}