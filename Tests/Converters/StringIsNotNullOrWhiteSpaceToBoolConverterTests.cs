using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class StringIsNotNullOrWhiteSpaceToBoolConverterTests
    {
        private static StringIsNotNullOrWhiteSpaceToBoolConverter converter = new StringIsNotNullOrWhiteSpaceToBoolConverter();

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_NullTest()
        {
            Assert.IsFalse((bool)converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_EmptyTest()
        {
            Assert.IsFalse((bool)converter.Convert(string.Empty, null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_PopulatedTest()
        {
            Assert.IsTrue((bool)converter.Convert("I am a self-aware string, oh my", null, null, null));
        }

        [TestMethod]
        public void StringIsNotNullOrWhiteSpaceToBoolConverter_NonStringTest()
        {
            Assert.IsFalse((bool)converter.Convert(DayOfWeek.Friday, null, null, null));
        }
    }
}
