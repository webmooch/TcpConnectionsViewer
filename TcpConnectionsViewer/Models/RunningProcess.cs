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

        public int Id { get; set; }
        public string ExecutablePath { get; set; }
        public string Caption { get; set; }
        public string CommandLine { get; set; }
        public string Owner { get; set; }
        public DateTime? CreationDate { get; set; }

        private TimeSpan? _timeSinceCreation;
        public TimeSpan? TimeSinceCreation
        {
            get
            {
                if (this.CreationDate != null)
                    _timeSinceCreation = DateTime.Now - this.CreationDate;
                return _timeSinceCreation;
            }
        }

        public static RunningProcess GetProcessByPid(int pid)
        {
            try
            {
                using (var processClassInstance = new ManagementObject("root\\CIMV2", string.Format("Win32_Process.Handle='{0}'", pid), null))
                {
                    return new RunningProcess()
                    {
                        Id = pid,
                        ExecutablePath = processClassInstance.Properties["ExecutablePath"].Value.ParseString(),
                        Caption = processClassInstance.Properties["Caption"].Value.ParseString(),
                        CommandLine = processClassInstance.Properties["CommandLine"].Value.ParseString(),
                        Owner = GetOwner(processClassInstance),
                        CreationDate = processClassInstance.Properties["CreationDate"].Value.ParseWMIDateTime()
                    };
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