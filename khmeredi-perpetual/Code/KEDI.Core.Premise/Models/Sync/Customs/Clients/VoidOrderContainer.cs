using CKBS.Models.Services.POS;
using KEDI.Core.Premise.Models.Services.POS;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sync.Customs.Clients
{
    public class VoidOrderContainer
    {
        public EntryMap<VoidOrder> VoidOrder { get; set; }
        public IEnumerable<EntryMap<VoidOrderDetail>> VoidOrderDetails { get; set; }
    }
}
