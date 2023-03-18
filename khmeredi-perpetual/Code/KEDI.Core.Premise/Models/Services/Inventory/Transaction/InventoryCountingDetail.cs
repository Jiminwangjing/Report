using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KEDI.Core.Premise.Models.Services.Inventory.Transaction
{
    [Table("tbInventoryCountingDetail",Schema ="dbo")]
    public class InventoryCountingDetail
    {
        [Key]
        public int ID { get; set; } 
        [NotMapped]
        public string LineID { get; set; }
        public int InventoryCountingID{ get; set; }
        public int EmployeeID { get; set; }
        public int ItemID { get; set; } 
        public int WarehouseID { get; set; } 
        public int UomID{get;set;}
        [NotMapped]
        public string Barcode { get; set; }
        [NotMapped]
        public string  ItemNo { get; set; }
        [NotMapped]
        public string  ItemName { get; set; }
        [NotMapped]
        public List<SelectListItem> Warehouse{ get; set; }
        public double InstockQty { get; set; }
        public bool Counted { get; set; }
        public double UomCountQty { get; set; }
        public double CountedQty { get; set; }
        public double Varaince { get; set; }
        public string UomName { get; set; }
        public string EmName { get; set; }
        public bool Delete{ get; set; }
       

    }
}