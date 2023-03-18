using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.General
{
    [Table("tbReceiptInformation", Schema ="dbo")]
    public class ReceiptInformation : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int? BranchID { get; set; }
        [Required(ErrorMessage ="Please input title !")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Please input address !")]
        public string Address { get; set; }
        [Required(ErrorMessage = "Please input tel !")]
        public string Tel1 { get; set; }
        [Required(ErrorMessage = "Please input tel !")]
        public string Tel2 { get; set; }
        [Required(ErrorMessage = "Please input description !")]
        public string KhmerDescription { get; set; }
        [Required(ErrorMessage = "Please input description !")]
        public string EnglishDescription { get; set; }
        public string Title2 { get; set; }
        public string Address2 { get; set; }
        public string Email { get; set; }
        public string VatTin { get; set; }
        
        public string PowerBy { get; set; }
        public string TeamCondition { get; set; }
        public string TeamCondition2 { get; set; }
        public string Website { get; set; }
        //[Required]
       // public string Language { get; set; }//Khmer,English, Chinese
        public string Logo { get; set; }
        [ForeignKey("BranchID")]
        public virtual Branch Branch { get; set; }

         //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }

    }
}
