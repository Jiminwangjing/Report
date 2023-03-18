
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Inventory
{
    [Table("tbUnitofMeasure",Schema ="dbo")]
    public class UnitofMeasure : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage ="Please input code !")]
        public string Code { get; set; }
        [Required(ErrorMessage ="Please input name !")]
        public string Name { get; set; }
        public string AltUomName { get; set; }
        public bool Delete { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
