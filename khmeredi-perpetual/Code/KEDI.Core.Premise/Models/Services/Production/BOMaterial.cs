using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Production
{
    [Table("tbBOMaterial", Schema = "dbo")]
    public class BOMaterial
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int BID { get; set; }
        [Required(ErrorMessage = "Please select Item !")]
        public int ItemID { get; set; }
        public int UserID { get; set; }
        [Required(ErrorMessage = "Please select Uom !")]
        public int UomID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostingDate { get; set; }
        public int SysCID { get; set; }
        public double TotalCost { get; set; }
        public bool Active { get; set; }
        //ForeignKey
        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("SysCID")]
        public Currency Currency { get; set; }
        public IEnumerable<BOMDetail> BOMDetails { get; set; }
    }
}
