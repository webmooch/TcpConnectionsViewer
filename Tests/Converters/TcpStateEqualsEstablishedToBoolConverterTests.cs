using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Net.NetworkInformation;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class TcpStateEqualsEstablishedToBoolConverterTests
    {
        private static TcpStateEqualsEstablishedToBoolConverter converter = new TcpStateEqualsEstablishedToBoolConverter();

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_TrueTest()
        {
            Assert.IsTrue((bool)converter.Convert(TcpState.Established, null, null, null));
        }

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_OtherStateTest()
        {
            Assert.IsFalse((bool)converter.Convert(TcpState.FinWait2, null, null, null));
        }

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_JunkTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(DayOfWeek.Friday, null, null, null));
        }

        [TestMethod]
        public void TcpStateEqualsEstablishedToBoolConverter_NullTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(null, null, null, null));
        }
    }
}