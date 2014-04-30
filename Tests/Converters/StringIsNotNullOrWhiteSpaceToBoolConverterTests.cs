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
    public class StringIsNotNullOrWhiteSpaceToBoolConverterTests
    {
        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_NullTest()
        {
            var converter = new StringIsNotNullOrWhiteSpaceToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_EmptyTest()
        {
            var converter = new StringIsNotNullOrWhiteSpaceToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(string.Empty, null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_PopulatedTest()
        {
            var converter = new StringIsNotNullOrWhiteSpaceToBoolConverter();
            Assert.IsTrue((bool)converter.Convert("I am a self-aware string, oh my", null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_NonStringTest()
        {
            var converter = new StringIsNotNullOrWhiteSpaceToBoolConverter();
            Assert.IsFalse((bool)converter.Convert(DayOfWeek.Friday, null, null, null));
        }
    }
}
