using CKBS.Models.Services.Administrator.Tables;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Inventory
{
    [Table("tbServiceData")]
    public class ServiceData
    {
        [Key]
        public int ID { get; set; }
        [NotMapped]
        public string LineMTID { get; set; }
        public int ServiceCallID { get; set; }
        public int ItemID { get; set; }
        [NotMapped]
        public string ItemCode { get; set; }
        [NotMapped]
        public string ItemName { get; set; }
        public string MfrSerialNo { get; set; }
        public string SerialNo { get; set; }
        public string PlateNumber { get; set; }
        public double Qty { get; set; }
        public List<ServiceItem> ServiceItems { get; set; }

    }
}
