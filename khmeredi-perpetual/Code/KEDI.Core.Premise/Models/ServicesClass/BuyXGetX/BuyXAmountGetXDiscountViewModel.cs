using CKBS.Models.Services.LoyaltyProgram.BuyXAmountGetXDiscount;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.BuyXGetX
{
    public class BuyXAmountGetXDiscountViewModel
    {
        public int ID { get; set; }
        public string LineID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public List<SelectListItem> PriceListSelect { get; set; }
        public int PriListID { get; set; }
        public DateTime DateF { get; set; }
        public DateTime DateT { get; set; }
        public DateTime StopDate { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Amount { get; set; }
        public string ItemName { get; set; }
        public List<SelectListItem> DisTypeSelect { get; set; }
        public decimal DisRateValue { get; set; }
        public bool Active { get; set; }
        public string PriList { get; set; }
        public TypeDiscountBuyXAmountGetXDiscount DisType { get; set; } //Rate = 1, Value=2
    }
}
