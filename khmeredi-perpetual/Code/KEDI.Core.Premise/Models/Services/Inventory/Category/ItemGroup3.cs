
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Category
{
    [Table("ItemGroup3",Schema ="dbo")]
    public class ItemGroup3 : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }

        [Required(ErrorMessage ="Please input name !"),Display(Name ="Item3 Name")]
        public string Name { get; set; }

        public bool Delete { get; set; }

        public string Images { get; set; }
     
        [Display(Name ="Item1 Name")]
        public int ItemG1ID { get; set; }
        [ForeignKey("ItemG1ID")]
        public  ItemGroup1 ItemGroup1 { get; set; }
        [Display(Name = "Item2 Name")]
        public int ItemG2ID { get; set; }
        [ForeignKey("ItemG2ID")]
        public  ItemGroup2 ItemGroup2 { get; set; }

        [Display(Name = "Color")]
        public int ColorID { get; set; }
        [ForeignKey("ColorID")]
        public Colors Colors { get; set; }

        [Display(Name = "Background")]
        public int? BackID { get; set; }
        [ForeignKey("BackID")]
        public Background Background { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
   
}
