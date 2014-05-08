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
        public ushort RemotePort { get; private set; }
        public int ProcessId { get; private set; }
        public TcpState State { get; private set; }
        public string PendingText { get; private set; }
        public string LoadingText { get; private set; }

        private object _remoteMacAddressAsyncLock = new object();
        private string _remoteMacAddressAsync;
        public string RemoteMacAddressAsync
        {
            get 
            {
                lock (_remoteMacAddressAsyncLock)
                {
                    return _remoteMacAddressAsync;
                }
            }
            set
            {
                lock (_remoteMacAddressAsyncLock)
                {
                    _remoteMacAddressAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _remoteMacAddressManufacturerAsyncLock = new object();
        private KnownMacManufacturer _remoteMacAddressManufacturerAsync;
        public KnownMacManufacturer RemoteMacAddressManufacturerAsync
        {
            get 
            {
                lock (_remoteMacAddressManufacturerAsyncLock)
                {
                    return _remoteMacAddressManufacturerAsync;
                }
            }
            set
            {
                lock (_remoteMacAddressManufacturerAsyncLock)
                {
                    _remoteMacAddressManufacturerAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _localPortCommonServiceAsyncLock = new object();
        private CommonService _localPortCommonServiceAsync;
        public CommonService LocalPortCommonServiceAsync
        {
            get 
            {
                lock (_localPortCommonServiceAsyncLock)
                {
                    return _localPortCommonServiceAsync;
                }
            }
            set
            {
                lock (_localPortCommonServiceAsyncLock)
                {
                    _localPortCommonServiceAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _remotePortCommonServiceAsyncLock = new object();
        private CommonService _remotePortCommonServiceAsync;
        public CommonService RemotePortCommonServiceAsync
        {
            get 
            {
                lock (_remotePortCommonServiceAsyncLock)
                {
                    return _remotePortCommonServiceAsync;
                }
            }
            set
            {
                lock (_remotePortCommonServiceAsyncLock)
                {
                    _remotePortCommonServiceAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _runningProcessAsyncLock = new object();
        private RunningProcess _runningProcessAsync;
        public RunningProcess RunningProcessAsync
        {
            get 
            {
                lock (_runningProcessAsyncLock)
                {
                    return _runningProcessAsync;
                }
            }
            set
            {
                lock (_runningProcessAsyncLock)
                {
                    _runningProcessAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _remoteHostnameAsyncLock = new object();
        private string _remoteHostnameAsync;
        public string RemoteHostnameAsync
        {
            get 
            {
                lock (_remoteHostnameAsyncLock)
                {
                    return _remoteHostnameAsync;
                }
            }
            set
            {
                lock (_remoteHostnameAsyncLock)
                {
                    _remoteHostnameAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _localHostnameAsyncLock = new object();
        private string _localHostnameAsync;
        public string LocalHostnameAsync
        {
            get 
            {
                lock (_localHostnameAsyncLock)
                {
                    return _localHostnameAsync;
                }
            }
            set
            {
                lock (_localHostnameAsyncLock)
                {
                    _localHostnameAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        private object _remoteGeoAsyncLock = new object();
        private GeoIpEntry _remoteGeoAsync;
        public GeoIpEntry RemoteGeoAsync
        {
            get 
            {
                lock (_remoteGeoAsyncLock)
                {
                    return _remoteGeoAsync;
                }
            }
            set
            {
                lock (_remoteGeoAsyncLock)
                {
                    _remoteGeoAsync = value;
                    OnPropertyChanged();
                }
            }
        }

        public TcpConnection(ushort localPort,
            string localAddress,
            ushort remotePort,
            string remoteAddress,
            int processId,
            TcpState state,
            string pendingText,
            string loadingText)
        {
            this.LocalPort = localPort;
            this.LocalAddress = localAddress;
            this.RemotePort = remotePort;
            this.RemoteAddress = remoteAddress;
            this.ProcessId = processId;
            this.State = state;
            this.PendingText = pendingText;
            this.LoadingText = loadingText;


            RemoteMacAddressAsync = PendingText;
            RemoteMacAddressManufacturerAsync = CreatePendingInstance<KnownMacManufacturer>();
            Task.Factory.StartNew(() =>
                {
                    RemoteMacAddressAsync = LoadingText;
                    var remoteMacAddress = IpHlpApi.ResolvePhysicalAddress(RemoteAddress);
                    RemoteMacAddressAsync = remoteMacAddress != null && !string.IsNullOrWhiteSpace(remoteMacAddress.ToString()) ? remoteMacAddress.ToString() : null;

                    RemoteMacAddressManufacturerAsync = CreateLoadingInstance<KnownMacManufacturer>();
                    RemoteMacAddressManufacturerAsync = KnownMacManufacturers.Instance.Lookup(RemoteMacAddressAsync);
                });


            LocalPortCommonServiceAsync = CreatePendingInstance<CommonService>();
            Task.Factory.StartNew(() =>
                {
                    LocalPortCommonServiceAsync = CreateLoadingInstance<CommonService>();
                    LocalPortCommonServiceAsync = CommonTcpServices.Instance.Lookup(LocalPort);
                });


            RemotePortCommonServiceAsync = CreatePendingInstance<CommonService>();
            Task.Factory.StartNew(() =>
                {
                    RemotePortCommonServiceAsync = CreateLoadingInstance<CommonService>();
                    RemotePortCommonServiceAsync = CommonTcpServices.Instance.Lookup(RemotePort);
                });


            RunningProcessAsync = CreatePendingInstance<RunningProcess>();
            Task.Factory.StartNew(() =>
                {
                    RunningProcessAsync = CreateLoadingInstance<RunningProcess>();
                    RunningProcessAsync = ProcessMonitor.Instance.GetProcessInfo(ProcessId);
                });


            RemoteHostnameAsync = PendingText;
            Task.Factory.StartNew(() =>
                {
                    RemoteHostnameAsync = LoadingText;
                    RemoteHostnameAsync = DnsHostCache.Instance.Resolve(IPAddress.Parse(RemoteAddress));
                });


            LocalHostnameAsync = PendingText;
            Task.Factory.StartNew(() =>
                {
                    LocalHostnameAsync = LoadingText;
                    LocalHostnameAsync = DnsHostCache.Instance.Resolve(IPAddress.Parse(LocalAddress));
                });


            RemoteGeoAsync = CreatePendingInstance<GeoIpEntry>();
            Task.Factory.StartNew(() =>
                {
                    RemoteGeoAsync = CreateLoadingInstance<GeoIpEntry>();
                    RemoteGeoAsync = GeoIpCache.Instance.Lookup(IPAddress.Parse(RemoteAddress));
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

        private T CreatePendingInstance<T>() where T : class, new()
        {
            var instance = CreateInstance<T>();
            return SetAllPublicStringPropertyValues<T>(instance, PendingText);
        }

        private T CreateLoadingInstance<T>() where T : class, new()
        {
            var instance = CreateInstance<T>();
            return SetAllPublicStringPropertyValues<T>(instance, LoadingText);
        }

        private static T CreateInstance<T>() where T : class, new()
        {
            return Activator.CreateInstance<T>();
        }

        private static T SetAllPublicStringPropertyValues<T>(T instance, string valueToSetAllPublicStringProperties) where T : class, new()
        {
            foreach (var property in instance.GetType().GetProperties())
            {
                if (property.PropertyType == typeof(string) && property.CanWrite && property.GetSetMethod().IsPublic)
                {
                    property.SetValue(instance, valueToSetAllPublicStringProperties);
                }
            }
            return instance;
        }
    }
}