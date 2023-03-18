using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CKBS.Models.Services.Production
{
    public class ItemDetails
    {
        public string LineID { get; set; }
        public int ItemID { get; set; }
        public int ID { get; set; }
        public int? UomID { get; set; }
        public int GuomID { get; set; }
        public string Code { get; set; }
        public int Qty { get; set; }
        public string Barcode { get; set; }
        public List<SelectListItem> UomSelect { get; set; }
        public string Uom { get; set; }
        public string Guom { get; set; }
        public double Cost { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Factor { get; set; }
        public decimal DisRate { get; set; }
        public decimal Amount { get; set; }

    }

    public class ItemGetdetails
    {
        public int MBID { get; set; }
        public int MItemID { get; set; }
        public int MUomID { get; set; }
        public DateTime PostingDate { get; set; }
        public double TotalCost { get; set; }
        public bool Active { get; set; }
        public int BID { get; set; }
        public int BDID { get; set; }
        public double Qty { get; set; }
        public double Cost { get; set; }
        public double Amount { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int GuomID { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string Uom { get; set; }
        public string Guom { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Factor { get; set; }
    }
}
