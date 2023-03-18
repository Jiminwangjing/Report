using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Inventory.Transaction
{
    [Table("tbTransfer",Schema ="dbo")]
    public class Transfer
    {
        [Key]
        public int TarmsferID { get; set; }
        public int WarehouseFromID { get; set; }
        public int WarehouseToID { get; set; }
        public int BranchID { get; set; }
        public int BranchToID { get; set; }
        public int UserID { get; set; }
        public int UserRequestID { get; set; }
        public string Number { get; set; }
        [Column(TypeName ="Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName ="Date")]
        public DateTime DocumentDate { get; set; }
        public string Remark { get; set; }
        public string Time { get; set; }
        public List<TransferDetail> TransferDetails { get; set; }
        [ForeignKey("WarehouseFromID")]
        public  Warehouse Warehouse { get; set; }
        [ForeignKey("WarehouseToID")]
        public Warehouse Warehouses { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAcount { get; set; }

        [ForeignKey("UserRequestID")]
        public UserAccount UserAccounts { get; set; }
        public int SysCurID { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int CompanyID { get; set; }
        public int SeriseID { get; set; }
        public int SeriseDID { get; set; }
        public int DocTypeID { get; set; }
        public int BaseOnID { get; set; }

    }
}
