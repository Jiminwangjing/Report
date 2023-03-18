using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.SignalR
{
    public class TableTime
    {
        public static char FREE = 'A';
        public static char LOCK = 'B';
        public int Index { get; set; }
        public int ID { get; set; }
        public string Time { get; set; }
        public char Status { get; set; }
    }
}
