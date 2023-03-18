using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.POS;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.Templates
{
    public class VoidItemReport
    {
        public int OrderID { get; set; }
        public string ID { get; set; }
        public string IDD { get; set; }
        public string OrderNo { get; set; }
        public string UserName { get; set; }
        public string DateIn { get; set; }
        public string DateOut { get; set; }
        public string DisTotal { get; set; }
        public string SubTotal { get; set; }
        public string GTotal { get; set; }
        public string Reason { get; set; }
        public string ItemCode { get; set; }
        public string EnglishName { get; set; }
        public string KhmerName { get; set; }
        public double Qty { get; set; }
        public string Uom { get; set; }
        public string UnitPrice { get; set; }
        public string DisItem { get; set; }
        public string Total { get; set; }
        public string SSoldAmount { get; set; }
        public string SDiscountItem { get; set; }
        public string SDiscountTotal { get; set; }
        public string SGrandTotal { get; set; }

    }
}
