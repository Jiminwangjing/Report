using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupGoodsReceiptStock
    {
        public string NumberNo { get; set; }
        public string PosDate { get; set; }
        public string DocDate { get; set; }
        public string Subtotal { get; set; }
        public List<Goods> Goods { get; set; }
        public GRHeader GRHeader { get; set; }
        public GRFooter GRFooter { get; set; }
    }
    public class Goods
    {
        public string NumberNo { get; set; }
        public string Barcode { get; set; }
        public string Code { get; set; }
        public string KhName { get; set; }
        public string EngName { get; set; }
        public string Qty { get; set; }
        public string Cost { get; set; }
        public string Uom { get; set; }
        public string ExpireDate { get; set; }
    }
    public class GRHeader
    {
        public string Logo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string Branch { get; set; }
        public string WareHouse { get; set; }
        public string EmpName { get; set; }
    }
    public class GRFooter
    {
        public string Currency { get; set; }
        public double SumGrandTotal { get; set; }
    }
}
