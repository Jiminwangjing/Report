using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace CKBS.Models.SignalR
{
    public class TableTimerTask
    {
        private TableTime tableTime;
        private CancellationTokenSource cancellationToken;

        public TableTime TableTime { get => tableTime; set => tableTime = value; }
        public CancellationTokenSource CancellationToken { get => cancellationToken; set => cancellationToken = value; }
    }
}
