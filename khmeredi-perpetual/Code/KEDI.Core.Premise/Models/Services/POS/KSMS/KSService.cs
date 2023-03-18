using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.POS.KSMS
{
    public class KSService
    {
        public int ID { get; set; }
        public int ReceiptID { get; set; }
        public int ReceiptDID { get; set; }
        public int KSServiceSetupId { get; set; }
        public int VehicleID { get; set; }
        public double Qty { get; set; }
        public double MaxCount { get; set; }
        public double UsedCount { get; set; }
        [NotMapped]
        public bool IsKsmsMaster { get; set; } = false;
        [NotMapped]
        public bool IsKsms { get; set; } = false;
        public int CusId { get; set; }
        public int PriceListID { get; set; }
    }
    public class KSServiceHistory
    {
        public int ID { get; set; }
        public int KSServiceMasterID { get; set; }
        public int KSServiceID { get; set; }
        public int ReceiptID { get; set; }
        public int ReceiptDID { get; set; }
        public double Qty { get; set; }
    }
}
