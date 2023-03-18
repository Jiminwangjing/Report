using CKBS.Models.Services.POS;
using CKBS.Models.Services.POS.KVMS;
using System.Collections;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sync.Customs.Clients
{
    public class ReceiptMemoContainer
    {
        public EntryMap<ReceiptMemo> ReceiptMemo { get; set; }
        public IEnumerable<EntryMap<ReceiptDetailMemo>> ReceiptDetailMemos { set; get; }
    }
}
