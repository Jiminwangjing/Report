using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Administrator.SystemInitialization;
using CKBS.Models.Services.ChartOfAccounts;
using CKBS.Models.Services.Financials;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.ServicesClass.SerialBatches.Sales;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.ServicesClass.KSMS
{
    public class CheckStockItemParams
    {
        public int RevenueAccID { get; set; }
        public int InventoryAccID { get; set; }
        public int COGSAccID { get; set; }
        public decimal RevenueAccAmount { get; set; }
        public decimal InventoryAccAmount { get; set; }
        public decimal COGSAccAmount { get; set; }
        public List<JournalEntryDetail> JournalEntryDetails { get; set; }
        public List<AccountBalance> AccountBalance { get; set; }
        public JournalEntry JournalEntry { get; set; }
        public DocumentType DocType{ get; set; }
        public DocumentType DouTypeID { get; set; }
        public Receipt Order { get; set; }
        public ReceiptDetail Item { get; set; }
        public string OffsetAcc { get; set; }
        public ItemMasterData ItemMaster { get; set; }
        public GroupDUoM Orft { get; set; }
        public OrderStatus OrderStaus { get; set; }
        public Company  Com { get; set; }
        public List<SerialNumber> Serials { get; set; }
        public List<BatchNo> Batches { get; set; }
    }
}
