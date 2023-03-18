using CKBS.Models.Services.Inventory;
using CKBS.Models.Services.Purchase;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Administrator.Inventory
{
    [Table("tbWarehouseDetail", Schema = "dbo")]
    public class WarehouseDetail
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
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
        public double InStock { get; set; }
        public int CurrencyID { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "0:yyyy-MM-dd")]
        public DateTime ExpireDate { get; set; }
        public int ItemID { get; set; }
        public double Cost { get; set; }
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
        public int BPID { get; set; }
        public DirectionSEBA Direction { get; set; }
        [NotMapped]
        public bool AutoCreate { get; set; }
        public int GRGIID { get; set; }
        public bool IsDeleted { get; set; }
        public int InStockFrom { get; set; }
        public int BaseOnID { get; set; }
        public PurCopyType PurCopyType { get; set; }
    }

    public class WareForAudiView
    {
        public double Cost { get; set; }
        public double Qty { get; set; }
        public DateTime ExpireDate { get; set; }
    }
}
