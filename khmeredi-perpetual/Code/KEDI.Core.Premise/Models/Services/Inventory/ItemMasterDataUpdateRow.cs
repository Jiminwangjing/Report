using Microsoft.AspNetCore.Http;

namespace CKBS.Models.Services.Inventory
{
    public class ItemMasterDataUpdateRow
    {
        public int ID { get; set; }
        public int InventoryUomID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public string Barcode { get; set; }
        public string Image { get; set; }
        public IFormFileCollection Files { get; set; }
    }
}
