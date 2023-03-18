using KEDI.Core.Premise.Models.Services.POS.Templates;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS.service
{
    public class ItemsReturn
    {
        public string LineID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string Uom { get; set; }
        public decimal InStock { get; set; }
        public decimal TotalStock { get; set; }
        public decimal OrderQty { get; set; }
        public decimal Committed { get; set; }
        public int ReceiptID { get; set; }
        public bool IsSerailBatch { get; set; }
        public bool IsBOM { get; set; }
    }

    public class ItemsReturnObj
    {
        public List<ItemsReturn> ItemsReturns { get; set; }
        public int ReceiptID { get; set; }
        public PrintInvoice PrintInvoice { get; set; }
        public bool PreviewReceipt { get; set; }
        public TypeOfSerialBatch TypeOfSerialBatch { set; get; }
        public object ItemSerialBatches { set; get; }
        public Dictionary<string, string> ErrorMessages { set; get; }
    }

    public enum TypeOfSerialBatch
    {
        None = 0,
        Serial = 1,
        Batch = 2
    }
}
