using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Administrator.Setup
{
    public enum FreightReceiptType
    {
        [Display(Name = "Service Charge(%)")]
        ServiceCharge,
        [Display(Name = "Manual (Amount)")]
        Manual
    }
    [Table("FreightReceipt")]
    public class FreightReceipt : ISyncEntity
    {
        [Key]
        public int ID { set; get; }
        [NotMapped]
        public string Name { set; get; }
        public FreightReceiptType FreightReceiptType { get; set; }
        [NotMapped]
        public List<SelectListItem> FreightReceiptTypes { get; set; }
        public int FreightID { set; get; }
        public int ReceiptID { set; get; }
        public decimal AmountReven { set; get; }
        public bool IsActive { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
    }
}
