using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ClassCopy
{
    [Table("CopyPurchaseOrder_from_Quotation")]
    public class PurchaesOrder_from_Quotation
    {
        [Key]
        public int ID { get; set; }
        public int WarehouseID { get; set; }
        public string Warehouse { get; set; }
        public int VendorID { get; set; }
        public string Vendor { get; set; }
        public string Reff_No { get; set; }
        public int LocalCurrencyID { get; set; }
        public int SystemCurrencyID { get; set; }
        public string SystemCurrency { get; set; }

        public string LocalCurrency { get; set; }
        public int UomID { get; set; }
        public string UomName { get; set; }
        public int UserID { get; set; }
        public string  User { get; set; }
        public string Invoice { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string RequiredDate { get; set; }
        public string ValidUntil { get; set; }
        public double Sub_Total { get; set; }
        public double Sub_Total_Sys { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValues { get; set; }
        public string TypeDis { get; set; }
        public double TaxRate { get; set; }
        public double TaxValues { get; set; }
        public double AppliedAmount { get; set; }
        public double Balance_Due { get; set; }
        public double Balance_Due_Sys { get; set; }
        public double ExchangRate { get; set; }
        public string Remark { get; set; }
        public string Status { get; set; }
        public int ItemID { get; set; }
        public int LineID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public double PurchasePrice { get; set; }
        public double Discount_Rate { get; set; }
        public double Discount_Values { get; set; }
        public string Type_Dis { get; set; }
        public double  Total { get; set; }
        public double Total_Sys { get; set; }
        public string ManageExpire { get; set; }
        public string ExpireDate { get; set; }
        public double AlertStock { get; set; }
        public string Barcode { get; set; }
        public int GroupUomID { get; set; }
        public int DetailQuotationID { get; set; }

    }
}
