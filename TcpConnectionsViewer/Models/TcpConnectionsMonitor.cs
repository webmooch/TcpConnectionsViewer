using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading;
using System.Windows.Threading;

namespace TcpConnectionsViewer.Models
{
    internal class TcpConnectionsMonitor : IDisposable
    {
        private readonly Dispatcher dispatcher;
        private BackgroundWorker backgroundWorker;

        public ObservableCollection<TcpConnection> TcpInfo { get; private set; }

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

        public TcpConnectionsMonitor(Dispatcher dispatcher, TimeSpan refreshInterval)
        {
            this.dispatcher = dispatcher;
            this.RefreshInterval = refreshInterval;
            TcpInfo = new ObservableCollection<TcpConnection>();
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
                        bool existingFound = false;
                        for (int j = 0; j < newConnections.Length; j++)
                        {
                            if (TcpInfo[i].Equals(newConnections[j]))
                            {
                                existingFound = true;
                                break;
                            }
                        }

                        if (!existingFound)
                        {
                            new Action(() => TcpInfo.RemoveAt(i)).ExecuteInSpecificThread(dispatcher);
                            i--;
                        }
                    }

                    // for each new entry that doesnt exist, add
                    for (int i = 0; i < newConnections.Length; i++)
                    {
                        bool existingFound = false;
                        for (int j = 0; j < TcpInfo.Count; j++)
                        {
                            if (TcpInfo[j].Equals(newConnections[i]))
                            {
                                existingFound = true;
                                break;
                            }
                        }
                        if (!existingFound)
                        {
                            var entry = new TcpConnection(newConnections[i].LocalPort, newConnections[i].LocalAddress.ToString(), newConnections[i].RemotePort, newConnections[i].RemoteAddress.ToString(), newConnections[i].dwOwningPid, newConnections[i].dwState, dispatcher);
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