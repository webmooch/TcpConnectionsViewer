using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class BoolToValueConverterTests
    {
        private static string trueValue = "True value";
        private static string falseValue = "False value";

        [TestMethod]
        public void BoolToValueConverter_TrueTest()
        {
            var converter = new BoolToValueConverter<string>() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(trueValue == (string)converter.Convert(true, null, null, null));
        }

        [TestMethod]
        public void BoolToValueConverter_FalseTest()
        {
            var converter = new BoolToValueConverter<string>() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(falseValue == (string)converter.Convert(false, null, null, null));
        }

        [TestMethod]
        public void BoolToValueConverter_NullTest()
        {
            var converter = new BoolToValueConverter<string>() { TrueValue = trueValue, FalseValue = falseValue };
            Assert.IsTrue(falseValue == (string)converter.Convert(null, null, null, null));
        }
    }
}