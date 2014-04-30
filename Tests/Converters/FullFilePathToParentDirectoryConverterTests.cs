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
    public class FullFilePathToParentDirectoryConverterTests
    {
        [TestMethod]
        public void FullFilePathToParentDirectoryConverter_Test()
        {
            var converter = new FullFilePathToParentDirectoryConverter();
            Assert.IsTrue(@"C:\temp" == (string)converter.Convert(@"C:\temp\file.file", null, null, null));
        }
    }
}
