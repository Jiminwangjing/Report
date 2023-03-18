using CKBS.Models.ServicesClass.Property;
using KEDI.Core.Premise.Models.Sale;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.POS.service
{
    [Table("ServiceItemSales", Schema = "dbo")]
    public class ServiceItemSales
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public int PromotionID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public int Group1 { get; set; }
        public int Group2 { get; set; }
        public int Group3 { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public float DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public double VAT { get; set; }

        public int CurrencyID { get; set; }
        public string Currency { get; set; }
        public int UomID { get; set; }
       
        public string UoM { get; set; }
        [NotMapped]
        public string Symbol { get; set; }
        [NotMapped]
        public DateTime StartDate { get; set; }
        [NotMapped]
        public DateTime StopDate { get; set; }

        public string Barcode { get; set; }
        public double InStock { get; set; }
        public string Process { get; set; }
        public string Image { get; set; }
        public int PricListID { get; set; }

        public int GroupUomID { get; set; }
        public string PrintTo { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public bool IsAddon { get; set; }

        public bool IsScale { set; get; }
        public int TaxGroupSaleID { get; set; }
        [NotMapped]
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
        [NotMapped]
        public List<SelectListItem> UomLists { get; set; }
        [NotMapped]
        public List<SelectListItem> UomChangeLists { get; set; }
        [NotMapped]
        public int Level { set; get; }


    }
}
