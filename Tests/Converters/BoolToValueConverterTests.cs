using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class BoolToValueConverterTests
    {
        private static string trueValue = "True value";
        private static string falseValue = "False value";
        private static BoolToValueConverter<string> converter = new BoolToValueConverter<string>() { TrueValue = trueValue, FalseValue = falseValue };

        [TestMethod]
        public void BoolToValueConverter_TrueTest()
        {
            Assert.IsTrue(trueValue == (string)converter.Convert(true, null, null, null));
        }

        [TestMethod]
        public void BoolToValueConverter_FalseTest()
        {
            Assert.IsTrue(falseValue == (string)converter.Convert(false, null, null, null));
        }

        [TestMethod]
        public void BoolToValueConverter_NullTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void BoolToValueConverter_JunkTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(DayOfWeek.Friday, null, null, null));
        }
    }
}