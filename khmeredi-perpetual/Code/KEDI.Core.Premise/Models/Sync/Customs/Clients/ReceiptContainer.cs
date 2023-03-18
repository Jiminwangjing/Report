using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS;
using System.Collections.Generic;
using System;
using CKBS.Models.Services.Administrator.Setup;

namespace KEDI.Core.Premise.Models.Sync.Customs.Clients
{
    public class ReceiptContainer
    {
        public EntryMap<Receipt> Receipt { set; get; }
        public IEnumerable<EntryMap<ReceiptDetail>> ReceiptDetails { set; get; }
        public IEnumerable<EntryMap<MultiPaymentMeans>> MultiPaymentMeans { set; get; }
        public IEnumerable<EntryMap<FreightReceipt>> FreightReceipts { set; get; }
    }
}
