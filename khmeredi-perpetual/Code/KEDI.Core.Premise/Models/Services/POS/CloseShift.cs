using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using KEDI.Core.Premise.Models.Sync;

namespace CKBS.Models.Services.POS
{
    [Table("tbCloseShift", Schema = "dbo")]
    public class CloseShift : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        public string TimeIn { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        public string TimeOut { get; set; }
        public int? BranchID { get; set; }
        public int UserID { get; set; }
        public double CashInAmount_Sys { get; set; }
        public double SaleAmount_Sys { get; set; }
        public int LocalCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public double LocalSetRate { get; set; }
        public double CashOutAmount_Sys { get; set; }//input from user cash
        public double ExchangeRate { get; set; }
        public double Trans_From { get; set; }
        public double Trans_To { get; set; }
        public bool Close { get; set; }
        public int OpenShiftID {get; set;}

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    public enum ShiftDateType
    {
        [Display(Name = "Close Shift")]
        CloseShift,
        [Display(Name = "Open Shift")]
        OpenShift
    }
}
