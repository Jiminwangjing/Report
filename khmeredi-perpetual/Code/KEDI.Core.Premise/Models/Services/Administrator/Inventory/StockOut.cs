using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Purchase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Administrator.Inventory
{
    [Table("StockOut")]
    public class StockOut
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
        public int OutStockFrom { get; set; }
        public int WarehouseID { get; set; }
        public int UomID { get; set; }
        public int UserID { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        [Column(TypeName = "Date")]
        public DateTime SyetemDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "0:hh:mm:ss")]

        public DateTime TimeIn { get; set; }
        public decimal InStock { get; set; }
        public int CurrencyID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        public DateTime? ExpireDate { get; set; }
        public int ItemID { get; set; }
        public decimal Cost { get; set; }
        public string MfrSerialNumber { get; set; }
        public string SerialNumber { get; set; }
        public string PlateNumber { get; set; }
        public string Color { get; set; }
        public string Brand { get; set; }
        public string Condition { get; set; }
        public string Type { get; set; }
        public string Power { get; set; }
        public string Year { get; set; }
        public string BatchNo { get; set; }
        public string BatchAttr1 { get; set; }
        public string BatchAttr2 { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? MfrDate { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? AdmissionDate { get; set; }
        public string Location { get; set; }
        public string Details { get; set; }
        public int SysNum { get; set; }
        public string LotNumber { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? MfrWarDateStart { get; set; }
        [Column(TypeName = "Date")]
        public DateTime? MfrWarDateEnd { get; set; }
        public TransTypeWD TransType { get; set; }
        public ProcessItem ProcessItem { get; set; }
        public DirectionSEBA Direction { get; set; }
        public int FromWareDetialID { get; set; }
        public int TransID { get; set; }
        public int Contract { get; set; }
        public int BPID { get; set; }
         public int BaseOnID { get; set; }
        public PurCopyType PurCopyType{get;set;}
    }
}
