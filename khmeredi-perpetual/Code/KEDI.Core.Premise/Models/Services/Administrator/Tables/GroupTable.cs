using CKBS.Models.Services.Administrator.General;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Tables
{
    [Table("tbGroupTable",Schema ="dbo")]
    public class GroupTable : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        [Required(ErrorMessage ="Please input name !")]
        public string Name { get; set; }
        [Required(ErrorMessage ="Please select Branch !")]
        public int BranchID { get; set; }
        [Required]
        public string Types { get; set; } = "Normal";//Normal,Time
        public string Image { get; set; }
        public bool Delete { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }
        [NotMapped]
        public List<Table> Tables { set; get; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
 
}
