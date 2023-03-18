using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using CKBS.Models.Services.POS;

namespace KEDI.Core.Premise.Models.Services.CustomerConsignments
{
    public enum LengthExpire
    {
        None = 0,
        [Display(Name = "1 week")]
        OneWeek = 1,
        [Display(Name = "2 week")]
        TwoWeek = 2,
    }

    public enum StatusSelect
    {
        Open = 1,
        Close = 2,
    }

    public class CustomerConsignment
    {
        [Key]
        public int CustomerConsignmentID { get; set; }
        public int CustomerID { get; set; }
        [NotMapped]
        public int SeriesID { get; set; }
        [NotMapped]
        public string ReceiptNo { get; set; }
        public int WarehouseID { get; set; }
        public int InvoiceID { get; set; }
        public LengthExpire LengthExpire { get; set; }
        [Column(TypeName = "Date")]
        public DateTime InvocieDate { get; set; }

        [Column(TypeName = "Date")]
        public DateTime ValidDate { get; set; }
        [NotMapped]
        public List<CusConsignmentDetail> ItemDetail { get; set; }
        public StatusSelect Status { get; set; }
    }

    public class CusConsignmentDetail
    {
        public int LineID { get; set; }
        public int DetailID { get; set; }
        public int ID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string KhmerName { get; set; }
        public double Qty { get; set; }
        public string Warehouse { get; set; }
    }
}