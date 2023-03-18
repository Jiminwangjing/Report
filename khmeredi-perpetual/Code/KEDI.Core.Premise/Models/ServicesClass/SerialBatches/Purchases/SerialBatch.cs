using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Purchases
{
    public class SerialViewModelPurchase
    {
        public string LineID { get; set; }
        public string DocNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string WhseCode { get; set; }
        public string WhseName { get; set; }
        public decimal TotalNeeded { get; set; }
        public decimal TotalCreated { get; set; }
        public decimal OpenQty { get; set; }
        public int ItemID { get; set; }
        public decimal Cost { get; set; }
        public int BaseOnID { get; set; }
        public int PurCopyType { get; set; }
        public bool Newrecord { get; set; }
        public List<SerialDetialViewModelPurchase> SerialDetialViewModelPurchase { get; set; }
        public List<AutomaticStringCreation> AutomaticStringCreations { get; set; }
        public List<AutomaticStringCreation> SerialAutomaticStringCreations { get; set; }
        public List<AutomaticStringCreation> LotAutomaticStringCreations { get; set; }
        public SerialDetailAutoCreation SerialDetailAutoCreation { get; set; }

    }
    public class BatchViewModelPurchase
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string DocNo { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string ItemDescription { get; set; }
        public string WhseCode { get; set; }
        public string WhseName { get; set; }
        public decimal TotalNeeded { get; set; }
        public decimal TotalCreated { get; set; }
        public List<BatchDetialViewModelPurchase> BatchDetialViewModelPurchases { get; set; }
        public List<AutomaticStringCreation> BatchStringCreations { get; set; }
        public List<AutomaticStringCreation> Batchtrr1StringCreations { get; set; }
        public List<AutomaticStringCreation> Batchtrr2StringCreations { get; set; }
        public BatchDetailAutoCreation BatchDetailAutoCreation { get; set; }
        public int ItemID { get; set; }
        public decimal Cost { get; set; }
    }

    public class SerialDetailAutoCreation
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public decimal Qty { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerailNumber { get; set; }
        public string PlateNumber { get; set; }
        public string LotNumber { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Year { get; set; }
        public string SysNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public DateTime? MfrWanStartDate { get; set; }
        public DateTime? MfrWanEndDate { get; set; }
        public string Location { get; set; }
        public string Detials { get; set; }
        public decimal Cost { get; set; }
        public List<AutomaticStringCreation> AutomaticStringCreations { get; set; }
        public List<AutomaticStringCreation> SerialAutomaticStringCreations { get; set; }
        public List<AutomaticStringCreation> LotAutomaticStringCreations { get; set; }
    }
    public class BatchDetailAutoCreation
    {
        public string LineID { get; set; } = DateTime.Now.Ticks.ToString();
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string WhsCode { get; set; }
        public decimal Qty { get; set; }
        public string Batch { get; set; }
        public decimal NoOfBatch { get; set; }
        public string BatchAtrr1 { get; set; }
        public string BatchAtrr2 { get; set; }
        public string SysNo { get; set; }
        public DateTime? AdmissionDate { get; set; }
        public DateTime? MfrDate { get; set; }
        public DateTime? ExpDate { get; set; }
        public string Location { get; set; }
        public string Detials { get; set; }
        public decimal Cost { get; set; }
        public List<AutomaticStringCreation> BatchStringCreations { get; set; }
        public List<AutomaticStringCreation> Batchtrr1StringCreations { get; set; }
        public List<AutomaticStringCreation> Batchtrr2StringCreations { get; set; }
    }
}
