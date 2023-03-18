using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints
{
    [Table("PointCard")]
    public class PointCard : ISyncEntity
    {
        [Key]
        public int ID { set; get; }
        public string LineID { set; get; }
        [Required]
        public string Code { set; get; }
        public string Title { set; get; }
        public int PointQty { set; get; }
        public decimal Amount { set; get; }
        public int PriceListID { set; get; }
        [NotMapped]
        public List<SelectListItem> PriceLists { set; get; }
        public DateTime DateFrom { set; get; }
        public DateTime DateTo { set; get; }
        public string RefNo { set; get; }
        public bool Active { set; get; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
