using System.ComponentModel.DataAnnotations;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using KEDI.Core.Premise.Models.Sync;

namespace KEDI.Core.Premise.Models.Integrations.ChipMong
{
    [Table("TransactionChipMong")]
    public class TransactionChipMong : ITransactionHistory
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public Guid RowId { set; get; }
        [Timestamp]
        public byte[] RowVersion { set; get; }
        public DateTime SyncDate { set; get; }
        public string TenantId { set; get; }
        public string PosId { set; get; }
        public string TxId { set; get; }
        public DateTimeOffset ChangeLog { get; set; }

    }
}
