using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX;
using KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram.ComboSale;
using KEDI.Core.Premise.Models.ServicesClass.BuyXGetX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram
{
    public class LoyaltyProgModel
    {
        public List<BuyXGetXDetailModel> BuyXGetXDetails { set; get; }
        public List<BusinessPartner> PointMembers { set; get; }
        public List<ComboSaleViewModel> ComboSales { get; set; }
        public List<BuyXAmountGetXDiscountViewModel> BuyXAmGetXDis { get; set; }
        public List<BuyXQtyGetXDisViewModel> BuyXQtyGetXDis { get; set; }
        public string Name {get;set;}

    }
}
