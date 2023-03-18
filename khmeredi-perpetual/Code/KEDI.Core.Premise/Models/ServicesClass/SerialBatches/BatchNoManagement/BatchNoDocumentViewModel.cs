﻿using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.BatchNoManagement
{
    public class BatchNoDocumentViewModel
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string DocNo { get; set; }
        public string ItemName1 { get; set; }
        public string ItemName2 { get; set; }
        public string ItemCode { get; set; }
        public string WhsCode { get; set; }
        public string WhsName { get; set; }
        public decimal Qty { get; set; }
        public decimal TotalCreated { get; set; }
        public decimal OpenQty { get; set; }
        public int ItemID { get; set; }
        public TransTypeWD TransType { get; set; }
        public int TransId { get; set; }
        public List<CreatedBatchNoViewModel> Batches { get; set; }
    }
}
