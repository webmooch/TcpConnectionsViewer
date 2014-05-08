using System;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Runtime.InteropServices;

namespace TcpConnectionsViewer.Models
{
    internal static class IpHlpApi
    {

        #region Declarations

        #region Enumerations

        #region MIB_IPNET_TYPE

        /// <summary>
        /// Defines the possible types of ARP entries.
        /// </summary>
        public enum MIB_IPNET_TYPE
        {
            /// <summary>
            /// Other
            /// </summary>
            OTHER = 0x1,

            /// <summary>
            /// An invalid ARP type. This can indicate an unreachable or incomplete ARP entry.
            /// </summary>
            INVALID = 0x2,

            /// <summary>
            /// A dynamic ARP type.
            /// </summary>
            DYNAMIC = 0x3,

            /// <summary>
            /// A static ARP type.
            /// </summary>
            STATIC = 0x4
        }

        #endregion

        #region TCP_TABLE_CLASS

        /// <summary>
        /// The TCP_TABLE_CLASS enumeration defines the set of values used to indicate the type of table returned by calls to GetExtendedTcpTable.
        /// </summary>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa366386%28v=vs.85%29.aspx
        /// </remarks>
        public enum TCP_TABLE_CLASS
        {
            /// <summary>
            /// A MIB_TCPTABLE table that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_BASIC_LISTENER,

            /// <summary>
            /// A MIB_TCPTABLE table that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_BASIC_CONNECTIONS,

            /// <summary>
            /// A MIB_TCPTABLE table that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_BASIC_ALL,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_PID_LISTENER,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID that structure that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_PID_CONNECTIONS,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_PID or MIB_TCP6TABLE_OWNER_PID structure that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_PID_ALL,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all listening (receiving only) TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_MODULE_LISTENER,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all connected TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_MODULE_CONNECTIONS,

            /// <summary>
            /// A MIB_TCPTABLE_OWNER_MODULE or MIB_TCP6TABLE_OWNER_MODULE structure that contains all TCP endpoints on the local computer is returned to the caller.
            /// </summary>
            TCP_TABLE_OWNER_MODULE_ALL
        }

        #endregion

        #endregion

        #region Structures

        #region MIB_IPNETROW

        /// <summary>
        /// The MIB_IPNETROW structure contains information for an Address Resolution Protocol (ARP) table entry for an IPv4 address.
        /// </summary>
        /// <example>
        /// typedef struct _MIB_IPNETROW {
        ///   DWORD dwIndex;
        ///   DWORD dwPhysAddrLen;
        ///   BYTE  bPhysAddr[MAXLEN_PHYSADDR];
        ///   DWORD dwAddr;
        ///   DWORD dwType;
        /// } MIB_IPNETROW, *PMIB_IPNETROW;
        /// </example>
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_IPNETROW
        {
            /// <summary>
            /// The index of the adapter.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Index;

            /// <summary>
            /// The length, in bytes, of the physical address.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int PhysAddrLen;

            /// <summary>
            /// The physical address. MAXLEN_PHYSADDR = 8
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
            public byte[] PhysAddr;

            /// <summary>
            /// The IPv4 address.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public int Addr;

            /// <summary>
            /// The type of ARP entry.
            /// </summary>
            [MarshalAs(UnmanagedType.U4)]
            public MIB_IPNET_TYPE Type;
        }

        #endregion

        #region MIB_TCPROW

        /// <summary>
        /// The MIB_TCPROW structure contains information that descibes an IPv4 TCP connection.
        /// </summary>
        public struct MIB_TCPROW
        {

            /// <summary>
            /// The state of the TCP connection.
            /// </summary>
            public TcpState State;

            /// <summary>
            /// The local IPv4 address for the TCP connection on the local computer. A value of zero indicates the listener can accept a connection on
            /// any interface.
            /// </summary>
            public uint LocalAddr;

            /// <summary>
            /// The local port number in network byte order for the TCP connection on the local computer.
            /// </summary>
            public uint LocalPort;

            /// <summary>
            /// The IPv4 address for the TCP connection on the remote computer. When the dwState member is MIB_TCP_STATE_LISTEN, this value has no meaning.
            /// </summary>
            public uint RemoteAddr;

            /// <summary>
            /// The remote port number in network byte order for the TCP connection on the remote computer. When the dwState member is MIB_TCP_STATE_LISTEN,
            /// this member has no meaning.
            /// </summary>
            public uint RemotePort;

        }

        #endregion

        #region MIB_TCPROW_OWNER_PID

        /// <summary>
        /// The MIB_TCPROW_OWNER_PID structure contains information that describes an IPv4 TCP connection with IPv4 addresses, ports used by the TCP connection, and the specific process ID (PID) associated with connection.
        /// </summary>
        /// <example>
        /// typedef struct _MIB_TCPROW_OWNER_PID {
        ///  DWORD dwState;
        ///  DWORD dwLocalAddr;
        ///  DWORD dwLocalPort;
        ///  DWORD dwRemoteAddr;
        ///  DWORD dwRemotePort;
        ///  DWORD dwOwningPid;
        /// } MIB_TCPROW_OWNER_PID, *PMIB_TCPROW_OWNER_PID;
        /// </example>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa366913%28v=vs.85%29.aspx
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            /// <summary>
            /// The state of the TCP connection. This member can be one of the values defined in the Iprtrmib.h header file. 
            /// </summary>
            public TcpState dwState;

            /// <summary>
            /// The local IPv4 address for the TCP connection on the local computer.
            /// A value of zero indicates the listener can accept a connection on any interface.
            /// </summary>
            public uint dwLocalAddr;

            /// <summary>
            /// The local port number in network byte order for the TCP connection on the local computer.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] dwLocalPort;

            /// <summary>
            /// The IPv4 address for the TCP connection on the remote computer.
            /// When the dwState member is MIB_TCP_STATE_LISTEN, this value has no meaning.
            /// </summary>
            public uint dwRemoteAddr;

            /// <summary>
            /// The remote port number in network byte order for the TCP connection on the remote computer.
            /// When the dwState member is MIB_TCP_STATE_LISTEN, this member has no meaning.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
            public byte[] dwRemotePort;

            /// <summary>
            /// The PID of the process that issued a context bind for this TCP connection. 
            /// </summary>
            public int dwOwningPid;


            public IPAddress LocalAddress
            {
                get
                {
                    return new IPAddress(dwLocalAddr);
                }
            }

            public ushort LocalPort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { dwLocalPort[1], dwLocalPort[0] }, 0);
                }
            }

            public IPAddress RemoteAddress
            {
                get
                {
                    return new IPAddress(dwRemoteAddr);
                }
            }

            public ushort RemotePort
            {
                get
                {
                    return BitConverter.ToUInt16(new byte[2] { dwRemotePort[1], dwRemotePort[0] }, 0);
                }
            }
        }

        #endregion

        #region MIB_TCPTABLE_OWNER_PID

        /// <summary>
        /// The MIB_TCPTABLE_OWNER_PID structure contains a table of process IDs (PIDs) and the IPv4 TCP links that are context bound to these PIDs.
        /// </summary>
        /// <example>
        /// typedef struct {
        ///   DWORD                dwNumEntries;
        ///   MIB_TCPROW_OWNER_PID table[ANY_SIZE];
        /// } MIB_TCPTABLE_OWNER_PID, *PMIB_TCPTABLE_OWNER_PID;
        /// </example>
        /// <remarks>
        /// http://msdn.microsoft.com/en-us/library/windows/desktop/aa366921%28v=vs.85%29.aspx
        /// </remarks>
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPTABLE_OWNER_PID
        {
            /// <summary>
            /// The number of MIB_TCPROW_OWNER_PID elements in the table.
            /// </summary>
            public uint dwNumEntries;

            /// <summary>
            /// Array of MIB_TCPROW_OWNER_PID structures returned by a call to GetExtendedTcpTable.
            /// </summary>
            MIB_TCPROW_OWNER_PID table;
        }

        #endregion

        #endregion

        #region Constants

        private const int ERROR_INSUFFICIENT_BUFFER = 122;

        #endregion

        #region Externals

        #region GetIpNetTable

        /// <summary>
        /// The GetIpNetTable function retrieves the IPv4 to physical address mapping table.
        /// </summary>
        ///<example>
        /// DWORD GetIpNetTable(
        ///   __out    PMIB_IPNETTABLE pIpNetTable,
        ///   __inout  PULONG pdwSize,
        ///   __in     BOOL bOrder
        /// );
        ///</example>
        /// <param name="pIpNetTable">A pointer to a buffer that receives the IPv4 to physical address mapping table as a MIB_IPNETTABLE structure.</param>
        /// <param name="pdwSize">On input, specifies the size in bytes of the buffer pointed to by the pIpNetTable parameter. On output, if the buffer is
        /// not large enough to hold the returned mapping table, the function sets this parameter equal to the required buffer size in bytes.</param>
        /// <param name="bOrder">A Boolean value that specifies whether the returned mapping table should be sorted in ascending order by IP address. If
        /// this parameter is TRUE, the table is sorted.</param>
        /// <returns>If the function succeeds, the return value is NO_ERROR or ERROR_NO_DATA. If the function fails or does not return any data, the return
        /// value is one of the following error codes:
        /// ERROR_INSUFFICIENT_BUFFER - The buffer pointed to by the pIpNetTable parameter is not large enough. The required size is returned in the DWORD
        /// variable pointed to by the pdwSize parameter.
        /// ERROR_INVALID_PARAMETER - An invalid parameter was passed to the function. This error is returned if the pdwSize parameter is NULL, or
        /// GetIpNetTable is unable to write to the memory pointed to by the pdwSize parameter.
        /// ERROR_NO_DATA - There is no data to return. The IPv4 to physical address mapping table is empty. This return value indicates that the call to
        /// the GetIpNetTable function succeeded, but there was no data to return.
        /// ERROR_NOT_SUPPORTED - The IPv4 transport is not configured on the local computer.
        /// Other - Use FormatMessage to obtain the message string for the returned error.</returns>
        /// <remarks>
        /// The GetIpNetTable function enumerates the Address Resolution Protocol (ARP) entries for IPv4 on a local system from the IPv4 to physical address
        /// mapping table and returns this information in a MIB_IPNETTABLE structure. The IPv4 address entries are returned in a MIB_IPNETTABLE structure in
        /// the buffer pointed to by the pIpNetTable parameter. The MIB_IPNETTABLE structure contains a count of ARP entries and an array of MIB_IPNETROW
        /// structures for each IPv4 address entry. Note that the returned MIB_IPNETTABLE structure pointed to by the pIpNetTable parameter may contain
        /// padding for alignment between the dwNumEntries member and the first MIB_IPNETROW array entry in the table member of the MIB_IPNETTABLE structure.
        /// Padding for alignment may also be present between the MIB_IPNETROW array entries. Any access to a MIB_IPNETROW array entry should assume padding
        /// may exist. on Windows Vista and later, the GetIpNetTable2 function can be used to retrieve the neighbor IP addresses for both IPv6 and IPv4. 
        /// </remarks>
        [DllImport("IpHlpApi.dll", EntryPoint = "GetIpNetTable", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.U4)]
        private static extern int GetIpNetTable(
            IntPtr pIpNetTable,
            [MarshalAs(UnmanagedType.U4)]
            ref int pdwSize,
            bool bOrder);

        #endregion

        #region GetTcpTable

        /// <summary>
        /// The GetTcpTable function retrieves the IPv4 TCP connection table.
        /// </summary>
        /// <example>
        /// DWORD WINAPI GetTcpTable(
        ///   __out    PMIB_TCPTABLE pTcpTable,
        ///   __inout  PDWORD pdwSize,
        ///   __in     BOOL bOrder
        /// );
        /// </example>
        /// <param name="pTcpTable">A pointer to a buffer that receives the TCP connection table as a MIB_TCPTABLE structure.</param>
        /// <param name="pdwSize">On input, specifies the size in bytes of the buffer pointed to by the pTcpTable parameter. On output, if
        /// the buffer is not large enough to hold the returned connection table, the function sets this parameter equal to the required
        /// buffer size in bytes.</param>
        /// <param name="bOrder">A Boolean value that specifies whether the TCP connection table should be sorted. If this parameter is TRUE,
        /// the table is sorted in the order of: (1) Local IP address; (2) Local port; (3) Remote IP address; (4) Remote port.</param>
        /// <returns>If the function succeeds, the return value is NO_ERROR.</returns>
        [DllImport("iphlpapi.dll", EntryPoint = "GetTcpTable")]
        private static extern int GetTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder);

        #endregion

        #region SetTcpEntry

        /// <summary>
        /// The SetTcpEntry function sets the state of a TCP connection.
        /// </summary>
        /// <example>
        /// DWORD SetTcpEntry(
        ///   __in  PMIB_TCPROW pTcpRow
        /// );
        /// </example>
        /// <param name="pTcpRow">A pointer to a MIB_TCPROW structure. This structure specifies information to identify the TCP connection to modify.
        /// It also specifies the new state for the TCP connection. The caller must specify values for all the members in this structure.</param>
        /// <returns>The function returns NO_ERROR (zero) if the function is successful.</returns>
        [DllImport("iphlpapi.dll", EntryPoint = "SetTcpEntry", SetLastError = true)]
        private static extern int SetTcpEntry(IntPtr pTcpRow);

        #endregion

        #region GetExtendedTcpTable

        /// <summary>
        /// The GetExtendedTcpTable function retrieves a table that contains a list of TCP endpoints available to the application.
        /// </summary>
        /// <example>
        /// DWORD GetExtendedTcpTable(
        ///  _Out_    PVOID pTcpTable,
        ///  _Inout_  PDWORD pdwSize,
        ///  _In_     BOOL bOrder,
        ///  _In_     ULONG ulAf,
        ///  _In_     TCP_TABLE_CLASS TableClass,
        ///  _In_     ULONG Reserved
        /// );
        /// </example>
        /// <param name="pTcpTable">
        /// A pointer to the table structure that contains the filtered TCP endpoints available to the application.
        /// For information about how to determine the type of table returned based on specific input parameter combinations, see the Remarks section later in this document.
        /// </param>
        /// <param name="pdwSize">
        /// The estimated size of the structure returned in pTcpTable, in bytes.
        /// If this value is set too small, ERROR_INSUFFICIENT_BUFFER is returned by this function, and this field will contain the correct size of the structure.
        /// </param>
        /// <param name="bOrder">
        /// A value that specifies whether the TCP connection table should be sorted. If this parameter is set to TRUE, the TCP endpoints in the table are sorted in ascending order, starting with the lowest local IP address. If this parameter is set to FALSE, the TCP endpoints in the table appear in the order in which they were retrieved.
        /// The following values are compared (as listed) when ordering the TCP endpoints:
        /// * Local IP address
        /// * Local scope ID (applicable when the ulAf parameter is set to AF_INET6)
        /// * Local TCP port
        /// * Remote IP address
        /// * Remote scope ID (applicable when the ulAf parameter is set to AF_INET6)
        /// * Remote TCP port
        /// </param>
        /// <param name="ulAf">
        /// The version of IP used by the TCP endpoints.
        /// Valid values:
        /// * AF_INET - IPv4 is used.
        /// * AF_INET6 - IPv6 is used.
        /// </param>
        /// <param name="TableClass">
        /// The type of the TCP table structure to retrieve. This parameter can be one of the values from the TCP_TABLE_CLASS enumeration.
        /// On the Windows SDK released for Windows Vista and later, the organization of header files has changed and the TCP_TABLE_CLASS enumeration is defined in the Iprtrmib.h header file, not in the Iphlpapi.h header file.
        /// The TCP_TABLE_CLASS enumeration value is combined with the value of the ulAf parameter to determine the extended TCP information to retrieve. 
        /// </param>
        /// <param name="Reserved">
        /// Reserved. This value must be zero.
        /// </param>
        /// <returns></returns>
        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern uint GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder, int ulAf, TCP_TABLE_CLASS TableClass, uint Reserved = 0);

        #endregion

        #endregion

        #endregion

        #region Methods

        #region GetTcpTableEx

        public static MIB_TCPROW_OWNER_PID[] GetTcpTableEx()
        {
            MIB_TCPROW_OWNER_PID[] tTable;
            int buffSize = 0;

            // Determine much memory is needed
            uint ret = GetExtendedTcpTable(IntPtr.Zero, ref buffSize, true, (int)AddressFamily.InterNetwork, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
            IntPtr buffTable = Marshal.AllocHGlobal(buffSize);

            try
            {
                ret = GetExtendedTcpTable(buffTable, ref buffSize, true, (int)(int)AddressFamily.InterNetwork, TCP_TABLE_CLASS.TCP_TABLE_OWNER_PID_ALL);
                if (ret != 0)
                {
                    return null;
                }

                // get the number of entries in the table
                MIB_TCPTABLE_OWNER_PID tab = (MIB_TCPTABLE_OWNER_PID)Marshal.PtrToStructure(buffTable, typeof(MIB_TCPTABLE_OWNER_PID));
                IntPtr rowPtr = (IntPtr)((long)buffTable + Marshal.SizeOf(tab.dwNumEntries));

                // buffer we will be returning
                tTable = new MIB_TCPROW_OWNER_PID[tab.dwNumEntries];

                for (int i = 0; i < tab.dwNumEntries; i++)
                {
                    MIB_TCPROW_OWNER_PID tcpRow = (MIB_TCPROW_OWNER_PID)Marshal.PtrToStructure(rowPtr, typeof(MIB_TCPROW_OWNER_PID));
                    tTable[i] = tcpRow;
                    rowPtr = (IntPtr)((long)rowPtr + Marshal.SizeOf(tcpRow));   // next entry
                }
            }
            finally
            {
                // Free the Memory
                Marshal.FreeHGlobal(buffTable);
            }
            return tTable;
        }

        #endregion

        #region CloseRemoteTcpConnection

        public static void CloseRemoteTcpConnection(string ipEndPoint)
        {
            int port = 0;
            if (ipEndPoint.Contains(':'))
            {
                string[] ipParts = ipEndPoint.Split(new char[] { ':' }, 2);
                ipEndPoint = ipParts[0];
                port = int.TryParse(ipParts[1], out port) ? port : 0;
            }
            IPAddress ipAddress = IPAddress.TryParse(ipEndPoint, out ipAddress) ? ipAddress : IPAddress.Any;
            CloseRemoteTcpConnection(new IPEndPoint(ipAddress, port));
        }

        public static void CloseRemoteTcpConnection(int port)
        {
            CloseRemoteTcpConnection(new IPEndPoint(0, port));
        }

        public static void CloseRemoteTcpConnection(IPAddress ipAddress)
        {
            CloseRemoteTcpConnection(new IPEndPoint(ipAddress, 0));
        }

        public static void CloseRemoteTcpConnection(IPEndPoint ipEndPoint)
        {
            uint ipAddress = BitConverter.ToUInt32(ipEndPoint.Address.GetAddressBytes(), 0);
            bool wildIpAddr = ipAddress == 0;
            bool wildPort = ipEndPoint.Port == 0;

            MIB_TCPROW[] rows = GetTcpTable();
            for (int i = 0; i < rows.Length; i++)
            {
                if (rows[i].State != TcpState.TimeWait)
                {
                    bool matchIpAddr = wildIpAddr || ipAddress == rows[i].RemoteAddr;
                    bool matchPort = wildPort || ipEndPoint.Port == IPAddress.HostToNetworkOrder((short)ipEndPoint.Port);

                    if (matchIpAddr && matchPort)
                    {
                        rows[i].State = TcpState.DeleteTcb;
                        SetTcpEntry(rows[i]);
                    }
                }
            }
        }

        #endregion

        #region GetIpNetTable

        /// <summary>
        /// Retrieves the IPv4 to physical address mapping table.
        /// </summary>
        /// <returns>The IPv4 to physical address mapping table.</returns>
        public static MIB_IPNETROW[] GetIpNetTable()
        {
            return GetIpNetTable(false);
        }

        /// <summary>
        ///  Retrieves the IPv4 to physical address mapping table.
        /// </summary>
        /// <param name="sort">Specifies whether the returned mapping table should be sorted in ascending order by IP address.</param>
        /// <returns>The IPv4 to physical address mapping table.</returns>
        public static MIB_IPNETROW[] GetIpNetTable(bool sort)
        {
            IntPtr pBuffer = IntPtr.Zero;
            try
            {
                // Used to hold the size of the returned data.
                int size = 0;

                // Call GetIpNetTable the first time to determine the size of the data to be returned.
                int result = GetIpNetTable(IntPtr.Zero, ref size, sort);

                // We should receive an 'Insufficient Buffer' error, otherwise throw an exception.
                if (result != ERROR_INSUFFICIENT_BUFFER) throw new Win32Exception(result);

                // Allocate the memory buffer of required size.
                pBuffer = Marshal.AllocCoTaskMem(size);

                // Call GetIpNetTable the second time to retreive the actual data.
                result = GetIpNetTable(pBuffer, ref size, sort);

                // If the result is not 0 (no error), then throw an exception.
                if (result != 0) throw new Win32Exception(result);

                // Read the first four bytes from the buffer to determine the number of MIB_IPNETROW structures returned.
                int count = Marshal.ReadInt32(pBuffer);

                //Declare an array to contain the returned data.
                MIB_IPNETROW[] ipNetTable = new MIB_IPNETROW[count];

                // Get the structure type and size.
                Type tMIB_IPNETROW = typeof(MIB_IPNETROW);
                size = Marshal.SizeOf(tMIB_IPNETROW);
                int dw = Marshal.SizeOf(new int()); // 4-bytes

                // Cycle through the entries.
                for (int index = 0; index < count; index++)
                {
                    // Call PtrToStructure, getting the structure information.
                    ipNetTable[index] = (MIB_IPNETROW)Marshal.PtrToStructure(new IntPtr(pBuffer.ToInt64() + dw + (index * size)), tMIB_IPNETROW);
                }
                return ipNetTable;
            }
            finally
            {
                // Release the allocate the memory buffer.
                if (pBuffer != IntPtr.Zero) Marshal.FreeCoTaskMem(pBuffer);
            }
        }

        #endregion

        #region GetTcpTable

        /// <summary>
        /// The GetTcpTable function retrieves the IPv4 TCP connection table.
        /// </summary>
        /// <returns>Returns the TCP connection table as an MIB_TCPROW array.</returns>
        public static MIB_TCPROW[] GetTcpTable()
        {
            return GetTcpTable(false);
        }

        /// <summary>
        /// The GetTcpTable function retrieves the IPv4 TCP connection table.
        /// </summary>
        /// <param name="sort">Specifies whether the TCP connection table should be sorted in the order of:
        /// (1) Local IP address
        /// (2) Local port
        /// (3) Remote IP address
        /// (4) Remote port
        /// </param>
        /// <returns>Returns the TCP connection table as an MIB_TCPROW array.</returns>
        public static MIB_TCPROW[] GetTcpTable(bool sort)
        {
            IntPtr pBuffer = IntPtr.Zero;
            try
            {
                // Used to hold the size of the returned data.
                int size = 0;

                // Call GetTcpTable the first time to determine the size of the data to be returned.
                int result = GetTcpTable(IntPtr.Zero, ref size, sort);

                // We should receive an 'Insufficient Buffer' error, otherwise throw an exception.
                if (result != ERROR_INSUFFICIENT_BUFFER) throw new Win32Exception(result);

                // Allocate the memory buffer of required size.
                pBuffer = Marshal.AllocCoTaskMem(size);

                // Call GetIpNetTable the second time to retreive the actual data.
                result = GetTcpTable(pBuffer, ref size, sort);

                // If the result is not 0 (no error), then throw an exception.
                if (result != 0) throw new Win32Exception(result);

                // Read the first four bytes from the buffer to determine the number of MIB_TCPROW structures returned.
                int count = Marshal.ReadInt32(pBuffer);

                //Declare an array to contain the returned data.
                MIB_TCPROW[] tcpTable = new MIB_TCPROW[count];

                // Get the structure type and size.
                Type tMIB_TCPROW = typeof(MIB_TCPROW);
                size = Marshal.SizeOf(tMIB_TCPROW);
                int dw = Marshal.SizeOf(new int()); // 4-bytes

                // Cycle through the entries.
                for (int index = 0; index < count; index++)
                {
                    // Call PtrToStructure, getting the structure information.
                    tcpTable[index] = (MIB_TCPROW)Marshal.PtrToStructure(new IntPtr(pBuffer.ToInt64() + dw + (index * size)), tMIB_TCPROW);
                }
                return tcpTable;
            }
            finally
            {
                // Release the allocate the memory buffer.
                if (pBuffer != IntPtr.Zero) Marshal.FreeCoTaskMem(pBuffer);
            }
        }

        #endregion

        #region ResolvePhysicalAddress

        /// <summary>
        /// Resolves an IPv4 Address to a physical (MAC) address. 
        /// </summary>
        /// <param name="ipAddress">The IPv4 Address string to resolve.</param>
        /// <returns>The corresponding Physical Address.</returns>
        public static PhysicalAddress ResolvePhysicalAddress(string ipAddress)
        {
            return ResolvePhysicalAddress(IPAddress.Parse(ipAddress));
        }

        /// <summary>
        /// Resolves an IPv4 Address to a physical (MAC) address. 
        /// </summary>
        /// <param name="ipAddress">The IPv4 Address object to resolve.</param>
        /// <returns>The corresponding Physical Address.</returns>
        public static PhysicalAddress ResolvePhysicalAddress(IPAddress ipAddress)
        {
            if (ipAddress.AddressFamily == AddressFamily.InterNetwork)
            {
                int address = BitConverter.ToInt32(ipAddress.GetAddressBytes(), 0);

                MIB_IPNETROW[] ipNetTable = GetIpNetTable();
                for (int i = 0; i < ipNetTable.Length; i++)
                {
                    if (ipNetTable[i].Addr == address)
                    {
                        byte[] mac = new byte[ipNetTable[i].PhysAddrLen];
                        Array.Copy(ipNetTable[i].PhysAddr, mac, mac.Length);
                        return new PhysicalAddress(mac);
                    }
                }
                return PhysicalAddress.None;
            }
            else
            {
                throw new Exception("Only IPv4 is supported.");
            }
        }

        #endregion

        #region SetTcpEntry

        public static void SetTcpEntry(MIB_TCPROW tcpRow)
        {
            IntPtr pTcpRow = IntPtr.Zero;
            try
            {
                pTcpRow = Marshal.AllocCoTaskMem(Marshal.SizeOf(tcpRow));
                Marshal.StructureToPtr(tcpRow, pTcpRow, false);
                int result = SetTcpEntry(pTcpRow);
                switch (result)
                {
                    case 0: break;
                    case -1: throw new Exception("Unsuccessful");
                    case 65: throw new Exception("User has insufficient privilege to execute this API successfully");
                    case 87: throw new Exception("Specified port is not in state to be closed down");
                    default: throw new Exception(string.Format("Unknown error '{0}' ({1})",result,  new Win32Exception(result).Message));
                }
            }
            finally
            {
                if (pTcpRow != IntPtr.Zero) Marshal.FreeCoTaskMem(pTcpRow);
            }
        }

        #endregion

        #endregion

    }
}