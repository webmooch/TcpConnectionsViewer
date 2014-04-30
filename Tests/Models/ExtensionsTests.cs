using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TcpConnectionsViewer.Models;

namespace Tests.Converters
{
    [TestClass]
    public class ExtensionsTests
    {
        [TestMethod]
        public void Extensions_OutOfMemoryExceptionIsCriticalTest()
        {
            var ex = new OutOfMemoryException();
            Assert.IsTrue(ex.IsCritical());
        }

        [TestMethod]
        public void Extensions_AppDomainUnloadedExceptionIsCriticalTest()
        {
            var ex = new AppDomainUnloadedException();
            Assert.IsTrue(ex.IsCritical());
        }

        [TestMethod]
        public void Extensions_BadImageFormatExceptionIsCriticalTest()
        {
            var ex = new BadImageFormatException();
            Assert.IsTrue(ex.IsCritical());
        }

        [TestMethod]
        public void Extensions_CannotUnloadAppDomainExceptionIsCriticalTest()
        {
            var ex = new CannotUnloadAppDomainException();
            Assert.IsTrue(ex.IsCritical());
        }

        [TestMethod]
        public void Extensions_InvalidProgramExceptionIsCriticalTest()
        {
            var ex = new InvalidProgramException();
            Assert.IsTrue(ex.IsCritical());
        }

        [TestMethod]
        public void Extensions_ThreadAbortExceptionIsCriticalTest()
        {
            var threadDelegate = new ThreadStart(() =>
                {
                    try
                    {
                        for (int i = 0; i < 1000; i++)
                        {
                            Thread.Sleep(10);
                        }
                    }
                    catch (ThreadAbortException ex)
                    {
                        Assert.IsTrue(ex.IsCritical());
                    }
                });
            var thread = new Thread(threadDelegate);
            thread.Start();
            Thread.Sleep(50);
            thread.Abort();
            Thread.Sleep(500);
        }

        [TestMethod]
        public void Extensions_TerminateProcessTest()
        {
            var psi = new ProcessStartInfo("cmd.exe") { WindowStyle = ProcessWindowStyle.Hidden };
            using (var process = Process.Start(psi))
            {
                Assert.IsNotNull(Process.GetProcessById(process.Id));
                process.Id.TerminateProcess();
                Thread.Sleep(100);
                Assert.IsTrue(process.HasExited);
            }
        }

        [TestMethod]
        public void Extensions_ParseStringNullTest()
        {
            object nullObject = null;
            Assert.IsNull(nullObject.ParseString());
        }

        [TestMethod]
        public void Extensions_ParseStringValidStringTest()
        {
            object stringObject = "valid string object";
            Assert.IsTrue(stringObject.ParseString() is string);
        }

        [TestMethod]
        public void Extensions_ParseStringValidObjectTest()
        {
            object obj = new object();
            Assert.IsTrue(obj.ParseString() == "System.Object");
        }

        [TestMethod]
        public void Extensions_ParseInt32EnumValidDayTest()
        {
            object dow = 5; // Best day of week
            Assert.IsTrue(dow.ParseInt32Enum<DayOfWeek>() == DayOfWeek.Friday);
        }

        [TestMethod]
        public void Extensions_ParseInt32EnumInvalidDayTest()
        {
            object dow = 7;
            Assert.IsNull(dow.ParseInt32Enum<DayOfWeek>());
        }

        [TestMethod]
        public void Extensions_ParseWMIDateTimeValidTest()
        {
            var wmiDt = "20140423115134.402877+600";
            var dt = wmiDt.ParseWMIDateTime();
            Assert.IsNotNull(dt);
            Assert.IsTrue(((DateTime)dt).Day == 23);
            Assert.IsTrue(((DateTime)dt).DayOfYear == 113);
            Assert.IsTrue(((DateTime)dt).DayOfWeek == DayOfWeek.Wednesday);
            Assert.IsTrue(((DateTime)dt).Year == 2014);
            Assert.IsTrue(((DateTime)dt).Millisecond == 402);
            Assert.IsTrue(((DateTime)dt).Minute == 51);
            Assert.IsTrue(((DateTime)dt).Second == 34);
        }

        [TestMethod]
        [ExpectedException(typeof(System.ArgumentOutOfRangeException))]
        public void Extensions_ParseWMIDateTimeOutOfRangeTest()
        {
            var wmiDt = "00000423115134.402877+000";
            var dt = wmiDt.ParseWMIDateTime();
        }

        [TestMethod]
        public void Extensions_ParseWMIDateTimeNullTest()
        {
            var wmiDt = "some garbage";
            var dt = wmiDt.ParseWMIDateTime();
            Assert.IsNull(dt);
        }
    }
}