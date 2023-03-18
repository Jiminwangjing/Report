using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupTransferStock
    {
        public string Number { get; set; }
        public string PosDate { get; set; }
        public string DocDate { get; set; }
        public string Time { get; set; }
        public string Subtotal { get; set; }
        public List<Goods> Goods { get; set; }
        public TFHeader TFHeader { get; set; }
        public GRFooter GRFooter { get; set; }
    }
    public class TFHeader
    {
        public string Logo { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public string BranchFrom { get; set; }
        public string BranchTo { get; set; }
        public string WHFrom { get; set; }
        public string WHTo { get; set; }
        public string EmpName { get; set; }
    }
}
