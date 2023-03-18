using CKBS.Models.Services.Inventory.Category;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.POS
{
    [Table("tbItemComment",Schema ="dbo")]
    public class ItemComment : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        [Required]
        public string Description { get; set; }
        public bool Deleted { set; get; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
