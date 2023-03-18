using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.POS.LoyaltyProgram
{
    [Table("Redeem")]
    public class Redeem : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public int CustomerID { get; set; }
       
        public int SeriesID { get; set; }
        public int UserID { get; set; }
        public int BranchID { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        public string Number { get; set; }
        public decimal RedeemPoint { get; set; }
        public int OpenShiftID { get; set; }
        public List<RedeemRetail> RedeemRetails { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
    // class View Redeem for Do Report Redeempoint
    public class RedeemView
    {
        public int ID { get; set; }
      
        public string DateOut { get; set; }
        public string RedeemCode { get; set; }
        public decimal RedeemPoint { get; set; }
        public decimal GTotalQty { get; set; }
        public decimal GTotalRedem { get; set; }
        public decimal TotalQty { get; set; }
        public string CusName { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public decimal Qty { get; set; }
        public string Uom { get; set; }
        
        public decimal TotalRedeempoint { get; set; }
                
    }
}
