using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace ModelTests
{
    [TestClass]
    public class ProcessMonitorTests
    {
        [TestMethod]
        public void ProcessMonitor_CreateProcessAndVerifyTest()
        {
            var pm = ProcessMonitor.Instance;
            var psi = new ProcessStartInfo("cmd.exe") { WindowStyle = ProcessWindowStyle.Hidden };
            using (var process = Process.Start(psi))
            {
                Assert.IsNotNull(Process.GetProcessById(process.Id));
                var d = pm.GetProcessInfo(process.Id);
                Assert.IsNotNull(d);
                Assert.IsTrue(new FileInfo(d.ExecutablePath).Name == "cmd.exe");
                process.Kill();
            }
        }
    }
}
