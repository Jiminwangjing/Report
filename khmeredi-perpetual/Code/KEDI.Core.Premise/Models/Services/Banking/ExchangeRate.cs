using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Banking 
{
    [Table("tbExchangeRate", Schema = "dbo")]
    public class ExchangeRate : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int CurrencyID { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currency { get; set; }
        public double Rate { get; set; }
        public bool Delete { get; set; }
        public double  RateOut { get; set; }
        public double SetRate { get; set; }
        public double DisplayRate { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
}
