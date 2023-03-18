using KEDI.Core.Premise.Models.Services.POS;
using System.Collections;
using System.Collections.Generic;

namespace KEDI.Core.Premise.Models.Sync.Customs.Clients
{
    public class VoidItemContainer
    {
        public EntryMap<VoidItem> VoidItem { get; set; }
        public IEnumerable<EntryMap<VoidItemDetail>> VoidItemDetails { get; set; }
    }
}
