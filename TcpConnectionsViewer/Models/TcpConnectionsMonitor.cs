using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows.Threading;

namespace TcpConnectionsViewer.Models
{
    internal class TcpConnectionsMonitor : IDisposable
    {
        private readonly Dispatcher dispatcher;
        private BackgroundWorker backgroundWorker;

        public ObservableCollection<TcpConnection> TcpInfo { get; private set; }
        public string AsyncPropertyPendingText { get; private set; }
        public string AsyncPropertyLoadingText { get; private set; }

        private TimeSpan _refreshInterval;
        private object _refreshIntervalLock = new object();
        public TimeSpan RefreshInterval
        {
            get
            {
                lock (_refreshIntervalLock)
                {
                    return _refreshInterval; 
                }
            }
            set
            {
                lock (_refreshIntervalLock)
                {
                    if (_refreshInterval.TotalMilliseconds > int.MaxValue)
                        throw new ArgumentOutOfRangeException(string.Format("Refresh interval duration cannot exceed {0}.", new TimeSpan(0, 0, 0, 0, int.MaxValue).ToString()));
                    
                    _refreshInterval = value;
                }
            }
        }

        public TcpConnectionsMonitor(Dispatcher dispatcher, TimeSpan refreshInterval, string asyncPropertyPendingText, string asyncPropertyLoadingText)
        {
            this.dispatcher = dispatcher;
            this.RefreshInterval = refreshInterval;
            this.TcpInfo = new ObservableCollection<TcpConnection>();
            this.AsyncPropertyPendingText = asyncPropertyPendingText;
            this.AsyncPropertyLoadingText = asyncPropertyLoadingText;
        }

        public void BeginMonitoringConnectionsAsync()
        {
            backgroundWorker = new BackgroundWorker();
            backgroundWorker.DoWork += (s, e) =>
            {
                while (true)
                {
                    var newConnections = IpHlpApi.GetTcpTableEx();

                    // for each old entry that doesnt exist in new list, remove
                    for (int i = 0; i < TcpInfo.Count; i++)
                    {
                        var existingFound = newConnections.Any(x => TcpInfo[i].Equals(x));
                        if (!existingFound)
                        {
                            new Action(() => TcpInfo.RemoveAt(i)).ExecuteInSpecificThread(dispatcher);
                            i--;
                        }
                    }

                    // for each new entry that doesnt exist, add
                    for (int i = 0; i < newConnections.Length; i++)
                    {
                        bool existingFound = TcpInfo.Any(x => x.Equals(newConnections[i]));
                        if (!existingFound)
                        {
                            var entry = new TcpConnection(newConnections[i].LocalPort,
                                newConnections[i].LocalAddress.ToString(),
                                newConnections[i].RemotePort,
                                newConnections[i].RemoteAddress.ToString(),
                                newConnections[i].dwOwningPid,
                                newConnections[i].dwState,
                                AsyncPropertyPendingText,
                                AsyncPropertyLoadingText);
                            new Action(() => TcpInfo.Add(entry)).ExecuteInSpecificThread(dispatcher);
                        }
                    }
                    Thread.Sleep((int)RefreshInterval.TotalMilliseconds);
                }
            };
            backgroundWorker.RunWorkerAsync();
        }

        public void Dispose()
        {
            if (backgroundWorker != null)
                backgroundWorker.Dispose();
        }
    }
}