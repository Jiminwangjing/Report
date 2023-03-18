using System;
using System.ComponentModel.DataAnnotations.Schema;
using KEDI.Core.Premise.Models.Sync;

namespace KEDI.Core.Premise.Models.Services.RemarkDiscount
{
    public class RemarkDiscountItem : ISyncEntity
    {
        public int ID { get; set; }
        public string Remark { get; set; }
        public bool Active { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
