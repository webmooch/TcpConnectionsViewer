using System.ComponentModel;
using System.Runtime.Serialization;

namespace TcpConnectionsViewer.Models
{
    /// <summary>
    /// http://www.telize.com/
    /// </summary>
    [DataContract]
    public class GeoIpEntry
    {
        [Description("Name of the country")]
        [DataMember(Name = "country")]
        public string Country { get; set; }

        [Description("DMA Code")]
        [DataMember(Name = "dma_code")]
        public string Dma_Code { get; set; }

        [Description("Time Zone")]
        [DataMember(Name = "timezone")]
        public string Timezone { get; set; }

        [Description("Area Code")]
        [DataMember(Name = "area_code")]
        public string Area_Code { get; set; }

        [Description("Visitor IP address, or IP address specified as parameter")]
        [DataMember(Name = "ip")]
        public string Ip { get; set; }

        [Description("Autonomous System Number")]
        [DataMember(Name = "asn")]
        public string Asn { get; set; }

        [Description("Two-letter continent code")]
        [DataMember(Name = "continent_code")]
        public string Continent_Code { get; set; }

        [Description("Internet service provider")]
        [DataMember(Name = "isp")]
        public string Isp { get; set; }
        
        [Description("Longitude")]
        [DataMember(Name = "longitude")]
        public string Longitude { get; set; } // is actually Nullable<double> - set to string for greater flexibility

        [Description("Latitude")]
        [DataMember(Name = "latitude")]
        public string Latitude { get; set; } // is actually Nullable<double> - set to string for greater flexibility

        [Description("Two-letter ISO 3166-1 alpha-2 country code")]
        [DataMember(Name = "country_code")]
        public string Country_Code { get; set; }

        [Description("Three-letter ISO 3166-1 alpha-3 country code")]
        [DataMember(Name = "country_code3")]
        public string Country_Code3 { get; set; }

        public override string ToString()
        {
            return Country;
        }
    }
}
