using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    public class ItemUoMView
    {
        public int ID { get; set; }
        public int ItemID { get; set; }
        public string Name { set; get; }
        public string KhmerName { get; set; }
        public string Code { get; set; }
    }
}
