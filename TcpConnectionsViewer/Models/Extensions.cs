using System;
using System.Diagnostics;
using System.IO;
using System.Management;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Threading;

namespace TcpConnectionsViewer.Models
{
    internal static class Extensions
    {
        public static bool IsCritical(this Exception ex)
        {
            if (ex is OutOfMemoryException) return true;
            if (ex is AppDomainUnloadedException) return true;
            if (ex is BadImageFormatException) return true;
            if (ex is CannotUnloadAppDomainException) return true;
            //if (ex is ExecutionEngineException) return true;
            if (ex is InvalidProgramException) return true;
            if (ex is ThreadAbortException) return true;
            return false;
        }

        public static void TerminateProcess(this int processId)
        {
            using (var process = Process.GetProcessById(processId))
            {
                process.Kill();
            }
        }

        public static string ParseString(this object value)
        {
            return (value != null) ? Convert.ToString(value) : default(string);
        }

        public static TEnum? ParseInt32Enum<TEnum>(this object value) where TEnum : struct
        {
            if (value != null)
            {
                if (value is byte || value is sbyte || value is short || value is ushort || value is int || value is uint || value is ulong || value is long)
                {
                    int convert;
                    if (int.TryParse(Convert.ToString(value), out convert))
                    {
                        if (Enum.IsDefined(typeof(TEnum), convert))
                        {
                            TEnum rtn;
                            return (Enum.TryParse<TEnum>(value.ToString(), out rtn)) ? rtn : default(TEnum?);
                        }
                    }
                }
            }
            return default(TEnum?);
        }

        public static DateTime? ParseWMIDateTime(this object value)
        {
            return (value != null && value is string && Regex.IsMatch((string)value, @"^\d{14}\.")) ? ManagementDateTimeConverter.ToDateTime(value.ToString()) : default(DateTime?);
        }

        public static T DeserializeAsJsonObject<T>(this string json) where T : class
        {
            var serializer = new DataContractJsonSerializer(typeof(T));
            using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(json)))
            {
                return serializer.ReadObject(ms) as T;
            }
        }

        public static void ExecuteInSpecificThread(this Action action, Dispatcher dispatcher)
        {
            if (dispatcher.CheckAccess())
                action.Invoke();
            else
                dispatcher.Invoke(DispatcherPriority.DataBind, action);
        }
    }
}