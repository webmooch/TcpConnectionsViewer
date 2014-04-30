using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class MultiBooleanToVisibilityConverterTests
    {
        [TestMethod]
        public void MultiBooleanToVisibilityConverter_AllTrueTest()
        {
            var converter = new MultiBooleanToVisibilityConverter();
            var allTrue = new object[] { true, true, true };
            Assert.IsTrue(Visibility.Visible == (Visibility)converter.Convert(allTrue, null, null, null));
        }

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_AllFalseTest()
        {
            var converter = new MultiBooleanToVisibilityConverter();
            var allFalse = new object[] { false, false, false };
            Assert.IsTrue(Visibility.Collapsed == (Visibility)converter.Convert(allFalse, null, null, null));
        }

        [TestMethod]
        public void MultiBooleanToVisibilityConverter_MixedTest()
        {
            var converter = new MultiBooleanToVisibilityConverter();
            var allFalse = new object[] { false, true, false };
            Assert.IsTrue(Visibility.Collapsed == (Visibility)converter.Convert(allFalse, null, null, null));
        }
    }
}