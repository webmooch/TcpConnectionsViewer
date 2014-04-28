using System;
using System.Collections.Concurrent;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace TcpConnectionsViewer.Models
{
    // TODO: Implement a way to terminate this class from running (IDisposable?)
    internal class ProcessMonitor
    {
        private static readonly Lazy<ProcessMonitor> lazy = new Lazy<ProcessMonitor>(() => new ProcessMonitor());

        public static ProcessMonitor Instance
        {
            get
            {
                return lazy.Value;
            }
        }

        private ConcurrentDictionary<int, RunningProcess> processes = new ConcurrentDictionary<int, RunningProcess>();
        private BackgroundWorker bg;

        private ProcessMonitor()
        {
            bg = new BackgroundWorker();

            bg.DoWork += (s, e) =>
            {
                while (true)
                {
                    var currentProcesses = Process.GetProcesses();

                    var newProcesses = currentProcesses.Where(x => !processes.ContainsKey(x.Id)).ToList();
                    for (int i = 0; i < newProcesses.Count; i++)
                    {
                        processes.TryAdd(newProcesses[i].Id, RunningProcess.GetProcessByPid(newProcesses[i].Id));
                        Debug.WriteLine("detected new process " + newProcesses[i].Id);
                        // TODO: raise event
                    }

                    var terminatedProcesses = processes.Where(x => !currentProcesses.Any(y => y.Id == x.Key)).ToList();
                    for (int i = 0; i < terminatedProcesses.Count; i++)
                    {
                        RunningProcess bleh;
                        processes.TryRemove(terminatedProcesses[i].Key, out bleh);
                        Debug.WriteLine("detected terminated process " + terminatedProcesses[i].Key);
                        // TODO: raise event
                    }
                    Thread.Sleep(100);
                }
            };

            bg.RunWorkerAsync();
        }

        // TODO: Improve waiting / queue system
        public RunningProcess GetProcessInfo(int pid)
        {
            int i = 0;
            while (i < 20)
            {
                if (processes.ContainsKey(pid))
                    return processes[pid];

                i++;
                Thread.Sleep(200);
            }
            return null;
        }
    }
}