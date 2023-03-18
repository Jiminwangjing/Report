using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    [Table("tbSeries", Schema = "dbo")]
    public class Series : ISyncEntity
    {
        [Key]
        public int ID { get; set; }        
        public string Name { get; set; }
        public string FirstNo { get; set; } 
        public string NextNo { get; set; }
        public string LastNo { get; set; }
        public string PreFix { get; set; }
        public int DocuTypeID { get; set; }
        public int PeriodIndID { get; set; }
        public bool Default { get; set; }
        public bool Lock { get; set; }
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
