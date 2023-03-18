using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    public class BuyXGetXModel
    {
        public BuyXGetX BuyXGetX { get; set; }
        public List<BuyXGetXDetail> BuyXGetXDetails { get; set; }
        public List<BuyXGetXDetailModel> BuyXGetXDetailModelView { get; set; } 
    }
}
