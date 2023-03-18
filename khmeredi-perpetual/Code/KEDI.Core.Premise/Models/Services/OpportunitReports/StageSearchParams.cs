using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.OpportunitReports
{
    public class StageSearchParams
    {
        public bool PurchaseOrder { get; set; }
        public bool GoodsReceiptPO { get; set; }
        public bool SaleAR { get; set; }
        public bool SaleQuotation { get; set; }
        public bool SaleOrder { get; set; }
        public bool SaleDelivery { get; set; }

    }
    public class searchbystage
    {
        public int ID { get; set; }
        public int StageID { get; set; }
    }
    public class StageSearchEmp
    {
        public bool Name { get; set; }
    }
    

}
