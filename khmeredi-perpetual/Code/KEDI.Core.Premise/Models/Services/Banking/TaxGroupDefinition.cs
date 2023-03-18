using KEDI.Core.Premise.Models.Sync;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Banking
{
    [Table("TaxDefinition", Schema = "dbo")]
    public class TaxGroupDefinition : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int TaxGroupID { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public decimal Rate { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
