using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sync;

namespace CKBS.Models.Services.POS
{
    [Table("tbOpenShift", Schema = "dbo")]
    public class OpenShift : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        public string TimeIn { get; set; }
        public int? BranchID { get; set; }
        public int? UserID { get; set; }
        public double CashAmount_Sys { get; set; }
        public double Trans_From { get; set; }
        public bool Open { get; set; }
        public int LocalCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public double LocalSetRate { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
