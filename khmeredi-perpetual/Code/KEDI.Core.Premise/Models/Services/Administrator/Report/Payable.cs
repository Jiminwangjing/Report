using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Report
{
    public class Payable
    {
        public int ID { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; }
    }
}
