using System;
using System.ComponentModel;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace TcpConnectionsViewer.Models
{
    internal class TcpConnection : INotifyPropertyChanged, IEquatable<TcpConnection>, IEquatable<IpHlpApi.MIB_TCPROW_OWNER_PID>
    {
        public string LocalAddress { get; private set; }
        public ushort LocalPort { get; private set; }
        public string RemoteAddress { get; private set; }
        public string RemoteMacAddress { get; private set; }
        public ushort RemotePort { get; private set; }
        public int ProcessId { get; private set; }
        public TcpState State { get; private set; }

        private string _remoteMacAddressManufacturer;
        public string RemoteMacAddressManufacturer
        {
            get { return _remoteMacAddressManufacturer; }
            set
            {
                _remoteMacAddressManufacturer = value;
                OnPropertyChanged();
            }
        }

        private CommonService _localPortCommonService;
        public CommonService LocalPortCommonService
        {
            get { return _localPortCommonService; }
            set
            {
                _localPortCommonService = value;
                OnPropertyChanged();
            }
        }

        private CommonService _remotePortCommonService;
        public CommonService RemotePortCommonService
        {
            get { return _remotePortCommonService; }
            set
            {
                _remotePortCommonService = value;
                OnPropertyChanged();
            }
        }

        private RunningProcess _runningProcess;
        public RunningProcess RunningProcess
        {
            get { return _runningProcess; }
            set
            {
                _runningProcess = value;
                OnPropertyChanged();
            }
        }

        private string _remoteHostname;
        public string RemoteHostname
        {
            get { return _remoteHostname; }
            set
            {
                _remoteHostname = value;
                OnPropertyChanged();
            }
        }

        private string _localHostname;
        public string LocalHostname
        {
            get { return _localHostname; }
            set
            {
                _localHostname = value;
                OnPropertyChanged();
            }
        }

        private GeoIpEntry _remoteGeo;
        public GeoIpEntry RemoteGeo
        {
            get { return _remoteGeo; }
            set
            {
                _remoteGeo = value;
                OnPropertyChanged();
            }
        }

        private readonly Dispatcher dispatcher;

        public TcpConnection(ushort localPort, string localAddress, ushort remotePort, string remoteAddress, int processId, TcpState state, Dispatcher dispatcher)
        {
            this.LocalPort = localPort;
            this.LocalAddress = localAddress;
            this.RemotePort = remotePort;
            this.RemoteAddress = remoteAddress;
            this.ProcessId = processId;
            this.State = state;
            this.dispatcher = dispatcher;

            var remoteMacAddress = IpHlpApi.ResolvePhysicalAddress(this.RemoteAddress);
            this.RemoteMacAddress = remoteMacAddress != null && !string.IsNullOrWhiteSpace(remoteMacAddress.ToString()) ? remoteMacAddress.ToString() : null;


            if (!string.IsNullOrWhiteSpace(this.RemoteMacAddress))
            {
                Task.Factory.StartNew(() =>
                {
                    var macManufacturer = KnownMacManufacturers.Instance.Lookup(this.RemoteMacAddress);
                    new Action(() => this.RemoteMacAddressManufacturer = macManufacturer.Name).ExecuteInSpecificThread(dispatcher);
                });
            }

            Task.Factory.StartNew(() =>
                {
                    var localPortCommonService = CommonTcpServices.Instance.Lookup(this.LocalPort);
                    new Action(() => this.LocalPortCommonService = localPortCommonService).ExecuteInSpecificThread(dispatcher);
                });

            Task.Factory.StartNew(() =>
                {
                    var remotePortCommonService = CommonTcpServices.Instance.Lookup(this.RemotePort);
                    new Action(() => this.RemotePortCommonService = remotePortCommonService).ExecuteInSpecificThread(dispatcher);
                });

            Task.Factory.StartNew(() =>
                {
                    var process = ProcessMonitor.Instance.GetProcessInfo(ProcessId);
                    new Action(() => this.RunningProcess = process).ExecuteInSpecificThread(dispatcher);
                });

            Task.Factory.StartNew(() =>
                {
                    var remoteHost = DnsHostCache.Instance.Resolve(IPAddress.Parse(this.RemoteAddress));
                    new Action(() => this.RemoteHostname = remoteHost).ExecuteInSpecificThread(dispatcher);
                });

            Task.Factory.StartNew(() =>
                {
                    var localHost = DnsHostCache.Instance.Resolve(IPAddress.Parse(this.LocalAddress));
                    new Action(() => this.LocalHostname = localHost).ExecuteInSpecificThread(dispatcher);
                });

            Task.Factory.StartNew(() =>
                {
                    var remoteGeo = GeoIpCache.Instance.Lookup(IPAddress.Parse(this.RemoteAddress));
                    new Action(() => this.RemoteGeo = remoteGeo).ExecuteInSpecificThread(dispatcher);
                });
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string caller = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(caller));
        }

        public bool Equals(TcpConnection obj)
        {
            if (obj == null)
                return false;

            if (this.LocalAddress != obj.LocalAddress)
                return false;

            if (this.LocalPort != obj.LocalPort)
                return false;

            if (this.RemoteAddress != obj.RemoteAddress)
                return false;

            if (this.RemotePort != obj.RemotePort)
                return false;

            if (this.State.ToString() != obj.State.ToString())
                return false;

            if (this.ProcessId != obj.ProcessId)
                return false;

            return true;
        }

        public bool Equals(IpHlpApi.MIB_TCPROW_OWNER_PID obj)
        {
            if (this.LocalAddress != obj.LocalAddress.ToString())
                return false;

            if (this.LocalPort != obj.LocalPort)
                return false;

            if (this.RemoteAddress != obj.RemoteAddress.ToString())
                return false;

            if (this.RemotePort != obj.RemotePort)
                return false;

            if (this.State.ToString() != obj.dwState.ToString())
                return false;

            if (this.ProcessId != obj.dwOwningPid)
                return false;

            return true;
        }

    }
}