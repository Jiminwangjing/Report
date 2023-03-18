using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Sale
{
    public class TpExchange
    {
        public int ID { get; set; }
        public int CurID { get; set; }
        public string CurName { get; set; }
        public double Rate { get; set; }
        public double SetRate { get; set; }
    }
}
