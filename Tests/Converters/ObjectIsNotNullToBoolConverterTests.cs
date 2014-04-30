using Microsoft.VisualStudio.TestTools.UnitTesting;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class ObjectIsNotNullToBoolConverterTests
    {
        private static ObjectIsNotNullToBoolConverter converter = new ObjectIsNotNullToBoolConverter();

        [TestMethod]
        public void ObjectIsNotNullToBoolConverter_TrueTest()
        {
            var obj = new object();
            Assert.IsTrue((bool)converter.Convert(obj, null, null, null));
        }

        [TestMethod]
        public void ObjectIsNotNullToBoolConverter_FalseTest()
        {
            Assert.IsFalse((bool)converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void ObjectIsNotNullToBoolConverter_EmptyStringTest()
        {
            Assert.IsTrue((bool)converter.Convert(string.Empty, null, null, null));
        }
    }
}