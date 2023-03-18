using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.HumanResources;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KEDI.Core.Premise.Models.Sale
{
    public enum HistoryOfSaleType
    {
        Quotation = 1, Order = 2, Delivery = 3, AR = 4, CreditMemo = 5, ReturnDelivery = 6, ARDownPayment = 7,
        ARReserve = 8,
        AREdite = 9,
        ARReEDT = 10,
    }
    public enum SaleDateType { PosingDate = 1, DocumentDate = 2, DueDate = 3 }
    public enum VatType
    {
        Default,
        Template01,
        Template02,
        Template03,
        Template04,
        Template05,
    }

    public enum VatTypeB
    {
        Default,
        [Display(Name = "None Vat")]
        NoneVat,
        Vat,
    }
    public enum PVatType
    {
        Defualt,
        Template01,
    }
    public class HistoryOfSale
    {
        public int ID { get; set; }
        public string InvoiceNo { get; set; }
        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string BalanceDueLC { get; set; }
        public string BalanceDueSC { get; set; }
        public string ExchangeRate { get; set; }
        public string PostingDate { get; set; }
        public string DocumentDate { get; set; }
        public string DueDate { get; set; }
        public string Status { get; set; }
        public int ShippedBy { get; set; }

        public List<SelectListItem> VatType { get; set; }
        public List<SelectListItem> VatTypeB { get; set; }
        public int TypeVatNumber { get; set; }
        public int CustomerID { get; set; }
        public int WarehouseID { get; set; }
    }


    public class HistoryOfSaleViewModel
    {
        public List<BusinessPartner> Customers { get; set; }
        public List<Warehouse> Warhouses { get; set; }
        public List<HistoryOfSale> SaleHistories { get; set; }
        public string Templateurl { get; set; }
    }

    public class HistoryOfSaleFilter
    {
        public int Customer { get; set; }
        public int Warehouse { get; set; }
        public int ShippedBy { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public SaleDateType DateType { get; set; }
        public HistoryOfSaleType SaleType { get; set; }
        public bool Check { get; set; }
    }
}
