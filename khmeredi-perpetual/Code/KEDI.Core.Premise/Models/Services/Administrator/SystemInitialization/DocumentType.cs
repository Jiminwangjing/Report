using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.SystemInitialization
{
    [Table("tbDocumentType", Schema = "dbo")]
    public class DocumentType : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        [NotMapped]
        public string Document { get; set; }
        [NotMapped]
        public string DefaultSeries { get; set; }
        [NotMapped]
        public string FirstNo { get; set; }
        [NotMapped]
        public string NextNo { get; set; }
        [NotMapped]
        public string LastNo { get; set; }     
        [NotMapped]
        public int DocuTypeID { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
