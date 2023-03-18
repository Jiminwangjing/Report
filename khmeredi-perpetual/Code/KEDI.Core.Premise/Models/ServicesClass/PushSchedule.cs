using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass
{
    public class PushSchedule
    {
        public int INVCount { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
