
using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Category
{
    [Table("ItemGroup1",Schema ="dbo")]
    public class ItemGroup1 : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ItemG1ID { get; set;}

        [Required(ErrorMessage ="Please input name !"),Display(Name="Item Name")]
        public string Name { get;set;}

        public string Images { get; set; }

        public bool Delete { get; set; }
        public bool Visible { get; set; }
        public bool IsAddon { get; set; }

        [NotMapped]
        public IFormFile File { get; set; }

        [Display(Name ="Color")]
        public int? ColorID { get; set; }
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