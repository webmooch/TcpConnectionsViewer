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
    public class BoolToVisibilityConverterTests
    {
        private static Visibility trueValue = Visibility.Visible;
        private static Visibility falseValue = Visibility.Hidden;

        [TestMethod]
        public void BoolToVisibilityConverter_TrueTest()
        {
            var converter = new BoolToVisibilityConverter() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(trueValue == (Visibility)converter.Convert(true, null, null, null));
        }

        [TestMethod]
        public void BoolToVisibilityConverter_FalseTest()
        {
            var converter = new BoolToVisibilityConverter() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(falseValue == (Visibility)converter.Convert(false, null, null, null));
        }

        [TestMethod]
        public void BoolToVisibilityConverter_NullTest()
        {
            var converter = new BoolToVisibilityConverter() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(falseValue == (Visibility)converter.Convert(null, null, null, null));
        }
    }
}
