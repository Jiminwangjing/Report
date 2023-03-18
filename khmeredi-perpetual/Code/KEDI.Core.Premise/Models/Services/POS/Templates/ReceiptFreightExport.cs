using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Administrator.Setup;

namespace KEDI.Core.Premise
{
    public class ReceiptFreightExport
    {
        public int ID { set; get; }
        public string SeriesCode { get; set; }
        public string SeriesNumber { get; set; }
        public FreightReceiptType FreightReceiptType { get; set; }
        //public List<SelectListItem> FreightReceiptTypes { get; set; }
        public string FreightName { set; get; }
        
        public decimal AmountReven { set; get; }
        public bool IsActive { get; set; }
    }
}