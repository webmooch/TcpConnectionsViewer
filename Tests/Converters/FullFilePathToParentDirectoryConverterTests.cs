using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Windows;
using TcpConnectionsViewer.Converters;

namespace Tests.Converters
{
    [TestClass]
    public class FullFilePathToParentDirectoryConverterTests
    {
        private static FullFilePathToParentDirectoryConverter converter = new FullFilePathToParentDirectoryConverter();

        [TestMethod]
        public void FullFilePathToParentDirectoryConverter_ValidTest()
        {
            Assert.IsTrue(@"C:\temp" == (string)converter.Convert(@"C:\temp\file.file", null, null, null));
        }

        [TestMethod]
        public void FullFilePathToParentDirectoryConverter_NullTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(null, null, null, null));
        }

        [TestMethod]
        public void FullFilePathToParentDirectoryConverter_EmptyTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(string.Empty, null, null, null));
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FullFilePathToParentDirectoryConverter_InvalidCharsTest()
        {
            converter.Convert("@#$%^&*", null, null, null);
        }

        [TestMethod]
        public void FullFilePathToParentDirectoryConverter_RootLevelPathTest()
        {
            Assert.IsTrue(DependencyProperty.UnsetValue == converter.Convert(@"C:\", null, null, null));
        }
    }
}
