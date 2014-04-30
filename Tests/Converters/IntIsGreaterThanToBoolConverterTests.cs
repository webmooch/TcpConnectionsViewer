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
    public class IntIsGreaterThanToBoolConverterTests
    {
        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_TrueTest()
        {
            var converter = new IntIsGreaterThanToBoolConverter();
            Assert.IsTrue((bool)converter.Convert(6, null, 5, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_FalseTest()
        {
            var converter = new IntIsGreaterThanToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(5, null, 6, null));
        }

        [TestMethod]
        public void IntIsGreaterThanToBoolConverter_AreSameTest()
        {
            var converter = new IntIsGreaterThanToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(5, null, 5, null));
        }
    }
}