﻿using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.HumanResources;
using KEDI.Core.Premise.Models.Services.Purchase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CKBS.Models.Services.Purchase
{
    [Table("tbPurchaseOrder", Schema = "dbo")]
    public class PurchaseOrder
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PurchaseOrderID { get; set; }
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        public int PurCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int DocumentTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public string ReffNo { get; set; }
        public string InvoiceNo { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PostingDate { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DocumentDate { get; set; }
        [DataType(DataType.Date)]
        [Column(TypeName = "Date")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DeliveryDate { get; set; }
        public double SubTotal { get; set; }
        public double SubTotalSys { get; set; }
        public decimal SubTotalAfterDis { get; set; }
        public decimal SubTotalAfterDisSys { get; set; }
        public double DiscountValue { get; set; }
        public double DiscountRate { get; set; }
        public string TypeDis { get; set; }
        public double TaxRate { get; set; }
        public double TaxValue { get; set; }
        public double BalanceDue { get; set; }
        public double PurRate { get; set; }
        public double BalanceDueSys { get; set; }
        public string Remark { get; set; }
        public double DownPayment { get; set; }
        public double DownPaymentSys { get; set; }
        public double AppliedAmount { get; set; }
        public double AppliedAmountSys { get; set; }
        public decimal FrieghtAmount { get; set; }
        public decimal FrieghtAmountSys{ get; set; }
        public double ReturnAmount { get; set; }
        public double AdditionalExpense { get; set; } // no calualte but save in database => textbox
        public string AdditionalNote { get; set; }// no calualte but save in database => textbox
        public List<PurchaseOrderDetail> PurchaseOrderDetails { get; set; }
        public string Status { get; set; }
        public double LocalSetRate  { get; set; }
        public int LocalCurID { get; set; }
        public int CompanyID { get; set; }
        public string Number { get; set; }
        public PurCopyType CopyType { get; set; }
        public string CopyKey { get; set; }
        public int BaseOnID { get; set; }
        [NotMapped]
        public FreightPurchase FreightPurchaseView { get; set; }
        //ForeignKey
        [ForeignKey("VendorID")]
        public BusinessPartner BusinessPartner { get; set; }
        [ForeignKey("WarehouseID")]
        public Warehouse Warehouse { get; set; }
        [ForeignKey("UserID")]
        public UserAccount UserAccount { get; set; }
        [ForeignKey("BranchID")]
        public Branch Branch { get; set; }
    }
}