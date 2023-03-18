using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.Inventory
{
    [Table("tbGroupDefindUoM",Schema ="dbo")]
    public class GroupDUoM : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int UoMID { get; set; }       
        public int GroupUoMID { get; set; }
        [Required(ErrorMessage ="Please input name !"),Column(TypeName ="decimal(18,2)")]
        public decimal AltQty { get; set; }
        [Required,Column(TypeName ="decimal(18,2)")]
        public decimal BaseQty { get; set; }
        [Required,Column(TypeName ="float")]
        
        public float Factor { get; set; }
        public int BaseUOM { get; set; }
        public int AltUOM { get; set; }
        public string Defined { get; set; } = "=";
        public bool Delete { get; set; } = false;

        [ForeignKey("UoMID"), Required, Display(Name = "BaseUoM")]
        public UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("GroupUoMID"), Required, Display(Name = "GroupUoM")]
        public GroupUOM GroupUOM { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
}
