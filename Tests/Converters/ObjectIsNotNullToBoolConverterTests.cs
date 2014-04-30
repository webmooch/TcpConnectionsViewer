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
    public class ObjectIsNotNullToBoolConverterTests
    {
        [TestMethod]
        public void ObjectIsNotNullToBoolConverter_TrueTest()
        {
            var converter = new ObjectIsNotNullToBoolConverter();
            var notNullObj = new object();
            Assert.IsTrue((bool)converter.Convert(notNullObj, null, null, null));
        }

        [TestMethod]
        public void ObjectIsNotNullToBoolConverter_FalseTest()
        {
            var converter = new ObjectIsNotNullToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(null, null, null, null));
        }
    }
}