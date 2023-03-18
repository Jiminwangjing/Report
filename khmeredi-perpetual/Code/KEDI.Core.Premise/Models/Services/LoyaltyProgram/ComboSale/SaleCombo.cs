using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.LoyaltyProgram.ComboSale
{
    public enum SaleType
    {
        [Display(Name = "Select")]
        None = 0,
        [Display(Name = "Sale-Parent")]
        SaleParent = 1,
        [Display(Name = "Sale-Child")]
        SaleChild = 2
    }
    [Table("SaleCombo")]
    public class SaleCombo : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int CreatorID { get; set; }
        [NotMapped]
        public string DateFormat { get; set; }
        public string Code { get; set; }
        public string Barcode { get; set; }
        [NotMapped]
        public string ItemName1 { get; set; }
        [NotMapped]
        public string ItemName2 { get; set; }
        [NotMapped]
        public string UoMName { get; set; }
        [NotMapped]
        public string TypeDisplay { get; set; }
        [NotMapped]
        public string PriceListName { get; set; }
        [NotMapped]
        public string Creator { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostingDate { get; set; }
        public int PriListID { get; set; }
        public bool Active { get; set; }
        public SaleType Type { get; set; }
        public IEnumerable<SaleComboDetail> ComboDetails { get; set; }
        //public int PBuyXAmountGetXDisID { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
