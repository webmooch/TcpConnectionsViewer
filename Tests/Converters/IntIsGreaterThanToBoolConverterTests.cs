using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class IntIsGreaterThanToBoolConverterTests
    {
        private static IntIsGreaterThanToBoolConverter converter = new IntIsGreaterThanToBoolConverter();

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_TrueTest()
        {
            Assert.IsTrue((bool)converter.Convert(6, null, 5, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_FalseTest()
        {
            Assert.IsFalse((bool)converter.Convert(5, null, 6, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_AreSameTest()
        {
            Assert.IsFalse((bool)converter.Convert(5, null, 5, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_JunkTest1()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(DayOfWeek.Friday, null, 5, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_JunkTest2()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(5, null, DayOfWeek.Friday, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_JunkTest3()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(DayOfWeek.Friday, null, DayOfWeek.Friday, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_NullTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(null, null, null, null));
        }
    }
}