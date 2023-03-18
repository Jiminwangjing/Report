using CKBS.Models.Services.Administrator.Inventory;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.Services.CustomerConsignments
{
    public enum StatusItem
    {
        None = 0,
        Withdraw = 1,
        Return = 2,
    }
    public class CustomerConsignmentDetail
    {
        [Key]
        public int CustomerConsignmentDetailID { get; set; }
        public int CustomerConsignmentID { get; set; }
        public int ItemID { get; set; }
        public double Qty { get; set; }
        public double OpenQty { get; set; }
        public int UomID { get; set; }
        public int GrpUomID { get; set; }
        public StatusItem Status { get; set; }
    }
}