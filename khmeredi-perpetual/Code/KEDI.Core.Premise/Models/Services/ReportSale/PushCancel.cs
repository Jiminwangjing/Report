using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Pagination;
using CKBS.Models.Services.POS.KVMS;
using CKBS.Models.Services.ReportSale.dev;

namespace KEDI.Core.Premise.Models.Services.ReportSale
{

    public class PushCancelForm
    {
        //public PushCancel[] PushCancels { set; get; }
        public Pagination<PushCancel> PageItems {set; get; }
    }
    public class PushCancel
    {
        public int ReceiptID { get; set; }
        public string ReceiptNo { get; set; } = "0";
        public DateTime DateOut { get; set; }
        public string TimeOut { get; set; }
        public double ExchangeRate { get; set; }
        public double DiscountValue { get; set; }
        public double GrandTotal { get; set; }
        public string EmpName { get; set; }
        public string LCCurrency { get; set; }
        public string PLCurrency { get; set; }
        public string SSCCurrency { get; set; }
        public double LSetRate { get; set; }
        public int WarehouseID {get;set;}
        public bool Selected { set; get; }
        public ReceiptDetailVeiw[] ReceiptDetailVeiws { get; set; }
        public double GrandTotalSC => GrandTotal * ExchangeRate;
        public double GrandTotalLC => GrandTotal * LSetRate;
    }
    public class ReceiptDetailVeiw
    {
        public int ID { get; set; }
        public int ReceiptID { get; set; }
        public int WarehouseID {get;set;}
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        public string KhmerName { get; set; }
        public string EnglisName { get; set; }
        public double Qty { get; set; }
        public string UoM { get; set; }
        public double UnitPrice { get; set; }
        public double DisValue { get; set; }
        public double Total { get; set; }
        public int UomID { get; set; }
        public double Factor { get; set; }

    }
}