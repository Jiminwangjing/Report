using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.ServicesClass.OpportunityView
{
    public class DocTypeData
    {
        public int ModuleID { get; set; }
        public int DoctypeID { get; set; }
        public string InvoiceNo { get; set; }
        public DateTime PostingDate { get; set; }
        public DateTime DueDate { get; set; }
        public string CustomerName { get; set; }
        public string Remark { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }

    }
}
