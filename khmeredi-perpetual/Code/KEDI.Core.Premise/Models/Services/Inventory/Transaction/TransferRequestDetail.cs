﻿using CKBS.Models.Services.Administrator.Inventory;
using CKBS.Models.Services.Banking;
using CKBS.Models.Services.Inventory;
using CKBS.Models.ServicesClass.Property;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.Inventory.Transaction
{
    public class TransferRequestDetail
    {
        [Key]
        public int ID { get; set; }
        public int TransferRequestID { get; set; }
        public int ItemID { get; set; }
        public int UomID { get; set; }
        public int LineID { get; set; }
        public int CurrencyID { get; set; }
        public int FWarehouseID { get; set; }
        public int TWarehouseID { get; set; }
        public string Code { get; set; }
        public string KhmerName { get; set; }
        public string EnglishName { get; set; }
        public double Qty { get; set; }
        public double OPenQty { get; set; }
        public double Cost { get; set; }
        public string Currency { get; set; }
        public string UomName { get; set; }
        public string Barcode { get; set; }
        public string Check { get; set; }
        public string ManageExpire { get; set; }
        [Column(TypeName = "Date")]
        public DateTime ExpireDate { get; set; }
        [ForeignKey("ItemID")]
        public ItemMasterData ItemMasterData { get; set; }
        [ForeignKey("UomID")]
        public UnitofMeasure UnitofMeasure { get; set; }
        [ForeignKey("CurrencyID")]
        public Currency Currencys { get; set; }
        [NotMapped]
        public int InventoryUoMID { get; set; }
        [NotMapped]
        public int UoMIDCheck { get; set; }
        [NotMapped]
        public int BaseOnID { get; set; }
        [NotMapped]
        public Dictionary<string, PropertydetailsViewModel> AddictionProps { get; set; }
    }
}
