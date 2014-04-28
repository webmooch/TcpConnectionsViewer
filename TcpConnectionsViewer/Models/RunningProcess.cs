using System;
using System.Diagnostics;
using System.Management;

namespace TcpConnectionsViewer.Models
{
    internal class RunningProcess
    {
        private enum GetOwnerReturnCode
        {
            Successful_Completion = 0,
            Access_Denied = 2,
            Insufficient_Privilege = 3,
            Unknown_Failure = 8,
            Path_Not_Found = 9,
            Invalid_Parameter = 21
        }

        public int Id { get; private set; }
        public string ExecutablePath { get; private set; }
        public string Caption { get; private set; }
        public string CommandLine { get; private set; }
        public string Owner { get; private set; }
        public DateTime? CreationDate { get; private set; }
        public TimeSpan? TimeSinceCreation { get; private set; }

        public RunningProcess(int processId, string executablePath, string caption, string commandLine, string owner, DateTime? creationDate)
        {
            this.Id = processId;
            this.ExecutablePath = executablePath;
            this.Caption = caption;
            this.CommandLine = commandLine;
            this.Owner = owner;
            this.CreationDate = creationDate;

            if (this.CreationDate != null)
                this.TimeSinceCreation = DateTime.Now - this.CreationDate;
        }

        public static RunningProcess GetProcessByPid(int pid)
        {
            try
            {
                using (var processClassInstance = new ManagementObject("root\\CIMV2", string.Format("Win32_Process.Handle='{0}'", pid), null))
                {
                    return new RunningProcess(
                        pid,
                        processClassInstance.Properties["ExecutablePath"].Value.ParseString(),
                        processClassInstance.Properties["Caption"].Value.ParseString(),
                        processClassInstance.Properties["CommandLine"].Value.ParseString(),
                        GetOwner(processClassInstance),
                        processClassInstance.Properties["CreationDate"].Value.ParseWMIDateTime()
                        );
                }
            }
            catch (Exception ex)
            {
                if (!ex.IsCritical())
                    return null;
                else
                    throw;
            }
        }

        /// <summary>
        /// http://msdn.microsoft.com/en-us/library/aa390460%28v=vs.85%29.aspx
        /// </summary>
        private static string GetOwner(ManagementObject processClassInstance)
        {
            try
            {
                using (var outParams = processClassInstance.InvokeMethod("GetOwner", null, null))
                {
                    var rtn = outParams["ReturnValue"].ParseInt32Enum<GetOwnerReturnCode>();
                    var domain = outParams["Domain"].ParseString();
                    var user = outParams["User"].ParseString();

                    if (rtn == GetOwnerReturnCode.Successful_Completion)
                        return !string.IsNullOrWhiteSpace(domain) ? string.Format(@"{0}\{1}", domain, user) : user;
                    else
                        Debug.WriteLine("Non successful process owner response: " + rtn.ToString());
                }
            }
            catch (ManagementException me)
            {
                Debug.WriteLine("ManagementException obtaining process owner: " + me.Message);
            }
            return null;
        }

        public override string ToString()
        {
            return this.Id.ToString();
        }
    }
}