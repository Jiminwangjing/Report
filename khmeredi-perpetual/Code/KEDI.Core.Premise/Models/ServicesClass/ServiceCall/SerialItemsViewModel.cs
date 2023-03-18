using CKBS.Models.Services.Administrator.Inventory;

namespace KEDI.Core.Premise.Models.ServicesClass.ServiceCall
{
    public class SerialItemsViewModel
    {
        public string LineID { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string MfrSerialNo { get; set; }
        public string Serial { get; set; }
        public string PlateNumber { get; set; }
        public string CustomerName { get; set; }
        public string Status { get; set; }
        public int ItemID { get; set; }
        public int GroupItemID { get; set; }
        public string ItemGroupName { get; set; }
    }
    public class BindARDeliveryPOS
    {
        public int CusId { get; set; }
        public int TransTypeId { get; set; }
        public TransTypeWD  TransType { get; set; }
    }
}
