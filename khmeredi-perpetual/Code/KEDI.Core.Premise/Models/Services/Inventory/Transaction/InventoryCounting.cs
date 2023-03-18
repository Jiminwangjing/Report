using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.Inventory;

namespace KEDI.Core.Premise.Models.Services.Inventory.Transaction
{
    [Table("tbInventoryCounting",Schema ="dbo")]
    public class InventoryCounting
    {
        [Key]
        public int ID { get; set; }
        public int DocTypeID { get; set; }
        public int BranchID { get; set; }
        public int SeriesID{get;set;}
        public int SeriesDetailID{get;set;}
        public string InvioceNumber { get; set; }
        public string Status { get; set; }
        public string Ref_No { get; set; }
        [Column(TypeName ="Date")]
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
        public string Remark{get;set;}
        public List<InventoryCountingDetail>InventoryCountingDetails{get;set;}
        public bool Delete{get;set;}

        
    }
}