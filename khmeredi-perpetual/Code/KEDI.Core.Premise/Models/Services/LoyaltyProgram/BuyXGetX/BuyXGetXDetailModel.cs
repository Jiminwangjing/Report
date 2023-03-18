using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    public class BuyXGetXDetailModel
    {
        public string LineID { get; set; }
        public int BuyItemID { get; set; }
        public string ProCode { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string BuyItemName { get; set; }
        public decimal BuyQty { get; set; }
        public string UoM { get; set; }
        public List<SelectListItem> ItemUoMs { get; set; }
        public int ItemUomID { get; set; }
        
        //Get Item
        public string Item { get; set; }
        public int GetItemID { get; set; }
        public string GetItemCode { get; set; }
        public string GetItemName { get; set; }
        public decimal GetQty { get; set; }
        public int GetUomID { get; set; }
        public string GetUomName { set; get; }
        public List<SelectListItem> PromoUoMs { get; set; }
        public int ID { get; set; }
        public int BuyXGetXID { get; set; }   

    }
}
