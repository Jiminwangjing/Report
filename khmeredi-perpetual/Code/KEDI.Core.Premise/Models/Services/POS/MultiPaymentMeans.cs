using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS
{
    [Table("MultiPaymentMean")]
    public class MultiPaymentMeans : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int ReceiptID { get; set; }
        public int PaymentMeanID { get; set; }
        public int AltCurrencyID { get; set; }
        public string AltCurrency { get; set; }
        public int AltRate { get; set; }
        public int PLCurrencyID { get; set; }
        public string PLCurrency { get; set; }
        public double PLRate { get; set; }
        [Column(TypeName = "decimal(29,11)")]
        public decimal Amount { get; set; }
        [Column(TypeName = "decimal(29,11)")]
        public decimal OpenAmount { get; set; }
        [Column(TypeName = "decimal(29,11)")]
        public decimal Total { get; set; }
        [Column(TypeName = "decimal(29,11)")]
        public decimal SCRate { get; set; }
        [Column(TypeName = "decimal(29,11)")]
        public decimal LCRate { get; set; }

        public bool ReturnStatus { get; set; }
        public PaymentMeanType Type { get; set; }
        public bool Exceed { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public enum PaymentMeanType
    {
        Normal = 0,
        CardMember = 1
    }
    

}
