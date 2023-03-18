using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    public class BuyXGetXView
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string PriceList { get; set; }
        public string DateF { get; set; }
        public string DateT { get; set; }
        public string StartTime { get; set; }
        public string StopTime { get; set; }

    }
}
