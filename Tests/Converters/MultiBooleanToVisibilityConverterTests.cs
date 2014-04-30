using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class MultiBooleanToVisibilityConverterTests
    {
        private static MultiBooleanToVisibilityConverter converter = new MultiBooleanToVisibilityConverter();

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_AllTrueTest()
        {
            var allTrue = new object[] { true, true, true };
            Assert.IsTrue(Visibility.Visible == (Visibility)converter.Convert(allTrue, null, null, null));
        }

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_AllFalseTest()
        {
            var allFalse = new object[] { false, false, false };
            Assert.IsTrue(Visibility.Collapsed == (Visibility)converter.Convert(allFalse, null, null, null));
        }

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_MixedTest()
        {
            var mixed = new object[] { true, true, false };
            Assert.IsTrue(Visibility.Collapsed == (Visibility)converter.Convert(mixed, null, null, null));
        }

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_JunkTest()
        {
            var allFalse = new object[] { true, true, DayOfWeek.Friday };
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(allFalse, null, null, null));
        }
    }
}
