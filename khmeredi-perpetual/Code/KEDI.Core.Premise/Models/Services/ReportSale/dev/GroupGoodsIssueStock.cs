using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.ReportSale.dev
{
    public class GroupGoodsIssueStock
    {
        public string NumberNo { get; set; }
        public string PosDate { get; set; }
        public string DocDate { get; set; }
        public string Subtotal { get; set; }
        public List<Goods> Goods { get; set; }
        public GRHeader GRHeader { get; set; }
        public GRFooter GRFooter { get; set; }
    }
}
