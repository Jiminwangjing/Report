using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.HumanResources
{ 
    public enum Status {Delete, Exist}
    [Table("tbQuoteAutoM", Schema = "dbo" )]
    public class QuoteAutoM
    {
        [Key]
        public int ID { get; set; }
        public string QNo { get; set; }
        public int CusID { get; set; } //BusinessPartner
        public string Code { get; set; }
        public string Name { get; set; }
        public int PriceListID { get; set; }
        public string Type { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        //AutoMobile
        public int AutoMID { get; set; } 
        public string Plate { get; set; }
        public string Frame { get; set; }
        public string Engine { get; set; }
        public string TypeName { get; set; }
        public string BrandName { get; set; }
        public string ModelName { get; set; }
        public string ColorName { get; set; }
        public string Year { get; set; }

        //VMC EDITION
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        //END VMC

        public Status Status { get; set; }
        public OrderQAutoM OrderQAutoM { get; set; }
    }

    [Table("tbOrderQAutoM", Schema = "dbo")]
    public class OrderQAutoM
    {
        [Key]
        public int OrderID { get; set; }
        public int QuoteAutoMID { get; set; }
        [Required]
        public string OrderNo { get; set; }
        [Required]
        public int TableID { get; set; }
        [Required]
        public string ReceiptNo { get; set; }
        [Required]
        public string QueueNo { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateIn { get; set; }
        [Required]
        [Column(TypeName = "Date")]
        public DateTime DateOut { get; set; }
        [Required]
        public string TimeIn { get; set; }
        [Required]
        public string TimeOut { get; set; }

        public int WaiterID { get; set; }
        [Required]
        public int UserOrderID { get; set; }
        [Required]
        public int UserDiscountID { get; set; }
        [Required]
        public int CustomerID { get; set; }
        [Required]
        public int CustomerCount { get; set; }
        [Required]
        public int PriceListID { get; set; }
        [Required]
        public int LocalCurrencyID { get; set; }
        [Required]
        public int SysCurrencyID { get; set; }
        [Required]
        public double ExchangeRate { get; set; }
        [Required]
        public int WarehouseID { get; set; }
        [Required]
        public int BranchID { get; set; }
        [Required]
        public int CompanyID { get; set; }
        [Required]
        public double Sub_Total { get; set; }
        [Required]
        public double DiscountRate { get; set; }
        [Required]
        public double DiscountValue { get; set; }
        [Required]
        public string TypeDis { get; set; } = "Percent";
        [Required]
        public double TaxRate { get; set; }
        [Required]
        public double TaxValue { get; set; }
        [Required]
        public double GrandTotal { get; set; }
        [Required]
        public double GrandTotal_Sys { get; set; }

        public double Tip { get; set; } = 0;
        [Required]
        public double Received { get; set; }
        [Required]
        public double Change { get; set; }
        public string CurrencyDisplay { get; set; }
        public double DisplayRate { get; set; }
        public double GrandTotal_Display { get; set; }
        public double Change_Display { get; set; }
        [Required]
        public int PaymentMeansID { get; set; }
        [Required]
        public char CheckBill { get; set; }
        public bool Cancel { get; set; } = false;
        public bool Delete { get; set; } = false;
        public int PLCurrencyID { get; set; }
        public double PLRate { get; set; }
        public double LocalSetRate { get; set; }
        public List<OrderDetailQAutoMs> OrderDetailQAutoMs { get; set; }

        [ForeignKey("LocalCurrencyID")]
        public Currency Currency { get; set; }
    }

    [Table("tbOrderDetailQAutoMs", Schema = "dbo")]
    public class OrderDetailQAutoMs
    {
        [Key]
        public int ID { get; set; }
        public int? OrderID { get; set; }
        public int Line_ID { get; set; }
        public int ItemID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Cost { get; set; }
        public double UnitPrice { get; set; }
        public double Qty { get; set; }
        public double PrintQty { get; set; }
        public int UomID { get; set; }
        public string Uom { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }
        public double Total { get; set; }
        public double Total_Sys { get; set; }
        public string ItemStatus { get; set; }//new,old
        public string ItemPrintTo { get; set; }
        public string Currency { get; set; }
        public string Comment { get; set; }
        public string ItemType { get; set; }
        public string Description { get; set; }
        public string ParentLevel { get; set; }//Line_ID+auto count line add on

        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeansure { get; set; }
    }
}
