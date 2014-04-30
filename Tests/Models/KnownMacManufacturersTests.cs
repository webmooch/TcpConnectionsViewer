using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace Tests.Converters
{
    [TestClass]
    public class KnownMacManufacturersTests
    {
        [TestMethod]
        public void KnownMacManufacturers_FirstKnownMacTest()
        {
            var response = KnownMacManufacturers.Instance.Lookup("000000");
            Assert.IsTrue(response.PrefixId == "000000");
            Assert.IsTrue(response.Name == "Officially Xerox but 000000 is more common");
        }

        [TestMethod]
        public void KnownMacManufacturers_LastKnownMacTest()
        {
            var response = KnownMacManufacturers.Instance.Lookup("FCFE77");
            Assert.IsTrue(response.PrefixId == "FCFE77");
            Assert.IsTrue(response.Name == "Hitachi Reftechno Inc.");
        }

        [TestMethod]
        public void KnownMacManufacturers_FakeMacTest()
        {
            var response = KnownMacManufacturers.Instance.Lookup("BEEEEF");
            Assert.IsNull(response);
        }

    }
}
