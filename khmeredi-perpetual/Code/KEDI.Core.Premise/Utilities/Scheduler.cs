using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace KEDI.Core.Utilities
{
    public interface IScheduler
    {
        event NotifyEventHandler Notified;
        event WatchEventHandler Watched;
    }

    public delegate void NotifyEventHandler(object sender, FileSystemEventArgs e);
    public delegate void WatchEventHandler(object sender, WatchEventArgs e); 
    public class WatchEventArgs : EventArgs
    {
        public DateTime WatchedTime { get; set; }
    }

    public class Scheduler : FileSystemWatcher, IScheduler
    {     
        public Scheduler() : base()
        {
            SetupCustomEventHandler();
        }
        public Scheduler(string path) : base(path)
        {
            SetupCustomEventHandler();
        }
        public Scheduler(string path, string filter) : base(path, filter)
        {
            SetupCustomEventHandler();
        }

        private static readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();
        public event NotifyEventHandler Notified;
        public event WatchEventHandler Watched;

        public int Interval { get; set; } = 1000;
        private void SetupCustomEventHandler()
        {
            if (Directory.Exists(Path))
            {
                NotifyFilter = NotifyFilters.Attributes
                             | NotifyFilters.CreationTime
                             | NotifyFilters.DirectoryName
                             | NotifyFilters.FileName
                             | NotifyFilters.LastAccess
                             | NotifyFilters.LastWrite
                             | NotifyFilters.Security
                             | NotifyFilters.Size;
                IncludeSubdirectories = true;
                EnableRaisingEvents = true;
                Created += (sender, e) => Notified?.Invoke(sender, e);
                Changed += (sender, e) => Notified?.Invoke(sender, e);
                Deleted += (sender, e) => Notified?.Invoke(sender, e);
                Renamed += (sender, e) => Notified?.Invoke(sender, e);
            }
            StartAsync(CancellationToken.None);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            return Task.Run(async() =>
            {
                var e = new WatchEventArgs { WatchedTime = DateTime.Now };
                do
                {
                    e.WatchedTime = DateTime.Now;
                    Watched?.Invoke(this, e);
                    await Task.Delay(Interval);
                } while (!cancellationToken.IsCancellationRequested);
            });
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _tokenSource.Cancel();
            return Task.CompletedTask;
        }
    }
}
