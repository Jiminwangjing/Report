using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    [Table("tbPeriodIndicator", Schema = "dbo")]
    public class PeriodIndicator : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public bool Delete { get; set; }
        public int CompanyID { get; set; }

         //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
