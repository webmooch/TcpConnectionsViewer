using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class TcpStateEqualsEstablishedToBoolConverterTests
    {
        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_TrueTest()
        {
            var converter = new TcpStateEqualsEstablishedToBoolConverter();
            Assert.IsTrue((bool)converter.Convert(TcpState.Established, null, null, null));
        }

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_OtherStateTest()
        {
            var converter = new TcpStateEqualsEstablishedToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(TcpState.FinWait2, null, null, null));
        }

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_InvalidArgTest()
        {
            var converter = new TcpStateEqualsEstablishedToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(DayOfWeek.Friday, null, null, null));
        }
    }
}