using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System;
using CKBS.Models.ServicesClass.Transfer;

namespace KEDI.Core.Premise.Models.Services.Inventory.Transaction
{
    public class TransferRequest
    {
        [Key]
        public int ID { get; set; }
        public int WarehouseFromID { get; set; }
        public int WarehouseToID { get; set; }
        public int BranchID { get; set; }
        public int BranchToID { get; set; }
        public int UserID { get; set; }
        public int UserRequestID { get; set; }
        public string Number { get; set; }
        [Column(TypeName = "Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime DocumentDate { get; set; }
        public string Remark { get; set; }
        public string Time { get; set; }
        
        public List<TransferRequestDetail> TransferRequestDetails { get; set; }
        [NotMapped]
        public List<TransferViewModel> TransferViewModels { get; set; }
        [ForeignKey("WarehouseFromID")]
        public Warehouse Warehouse { get; set; }
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
        public TranRequStatus TranRequStatus { get; set; }
    }
    public enum TranRequStatus
    { 
        Open=1,
        Colse= 2,
    }

}
