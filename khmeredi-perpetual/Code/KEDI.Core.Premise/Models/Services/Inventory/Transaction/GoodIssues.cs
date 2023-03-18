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
    [Table("tbGoodIssues",Schema ="dbo")]
    public class GoodIssues
    {
        [Key]
        public int GoodIssuesID { get; set; }
        public int BranchID { get; set; }
        public int UserID { get; set; }
        public int WarehouseID { get; set; }
        [Column(TypeName ="Date")]
        public DateTime PostingDate { get; set; }
        [Column(TypeName ="Date")]
        public DateTime DocumentDate { get; set; }
        public string Ref_No { get; set; }
        public string Number_No { get; set; }
        public string Remark { get; set; }
        public List<GoodIssuesDetail> GoodIssuesDetails { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAcount { get; set; }
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }
        public int SysCurID { get; set; }
        public int LocalCurID { get; set; }
        public double LocalSetRate { get; set; }
        public int CompanyID { get; set; }
        public int SeriseID { get; set; }
        public int SeriseDID { get; set; }
        public int DocTypeID { get; set; }
        public int GLID { get; set; }
    }
}
