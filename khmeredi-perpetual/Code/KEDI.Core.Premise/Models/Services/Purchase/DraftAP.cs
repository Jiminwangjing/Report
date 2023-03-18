﻿using CKBS.Models.Services.Account;
using CKBS.Models.Services.Administrator.General;
using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.HumanResources;
using CKBS.Models.Services.Purchase;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.Purchase
{
    [Table("DraftAP")]
    public class DraftAP
    {
        [Key]
        public int DraftID { get; set; }
        public int VendorID { get; set; }
        public int BranchID { get; set; }
        public int PurCurrencyID { get; set; }
        public int SysCurrencyID { get; set; } 
        public int WarehouseID { get; set; }
        public int UserID { get; set; }
        public int DocumentTypeID { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDetailID { get; set; }
        public int CompanyID { get; set; }
        public string Number { get; set; }
        public string ReffNo { get; set; }
        public string InvoiceNo { get; set; }
        public double PurRate { get; set; }
        public double BalanceDueSys { get; set; }
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
        public DateTime DueDate { get; set; }
        public double SubTotal { get; set; }
        public double SubTotalSys { get; set; }
        public decimal SubTotalAfterDis { get; set; }
        public decimal SubTotalAfterDisSys { get; set; }
        public decimal FrieghtAmount { get; set; }
        public decimal FrieghtAmountSys { get; set; }
        public double DiscountRate { get; set; }
        public double DiscountValue { get; set; }
        public string TypeDis { get; set; }//Percent ,cash
        public double TaxRate { get; set; }
        public double TaxValue { get; set; }
        public double DownPayment { get; set; }
        public double DownPaymentSys { get; set; }
        public double AppliedAmount { get; set; }
        public double AppliedAmountSys { get; set; }
        public double ReturnAmount { get; set; }
        public double BalanceDue { get; set; }
        public double AdditionalExpense { get; set; } // no calualte but save in database => textbox
        public string AdditionalNote { get; set; }// no calualte but save in database => textbox
        public string Remark { get; set; }
        public string DraftName { get; set; }
        public bool Remove { get; set; }
        public string Status { get; set; } //open,close 
        public double LocalSetRate { get; set; }
        public int LocalCurID { get; set; }
        public int BaseOnID { get; set; } 
        public List<DraftAPDetail> DraftDetails { get; set; }
        [NotMapped]
        public FreightPurchase FreightPurchaseView { get; set; }
    }
}