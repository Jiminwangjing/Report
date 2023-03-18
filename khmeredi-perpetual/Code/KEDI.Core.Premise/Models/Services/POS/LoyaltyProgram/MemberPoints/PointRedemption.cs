using KEDI.Core.Premise.Models.Sync;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.MemberPoints
{
    [Table("PointRedemptionMaster")]
    public class PointRedemptionMaster
    {
        public int ID { get; set; }
        public DateTime PostingDate { get; set; }
        public string Number { get; set; }
        public int UserID { get; set; }
        public int CustomerID { get; set; }
        public int PriceListID { get; set; }
        public int LocalCurrencyID { get; set; }
        public int SysCurrencyID { get; set; }
        public int WarehouseID { get; set; }
        public int BranchID { get; set; }
        public int CompanyID { get; set; }
        public bool Delete { get; set; } = false;
        public int PLCurrencyID { get; set; }
        public double PLRate { get; set; }
        public double LocalSetRate { get; set; }
        public int SeriesID { get; set; }
        public int SeriesDID { get; set; }
        public int DocTypeID { get; set; }
        public List<PointRedemptionHistory> PointRedemptions { get; set; }
    }

    public class PointRedemptionBase : ISyncEntity
    {
        [Key]
        public int ID { set; get; }

        public string Code { set; get; }
        //[Required]
        public string Title { set; get; }
        public double PointQty { set; get; }
        [DataType(DataType.Date)]
        public DateTime DateFrom { set; get; }
        [DataType(DataType.Date)]
        public DateTime DateTo { set; get; }
        public bool Active { set; get; }
        [NotMapped]
        public double Factor { get; set; } = 1;
        public bool Redeemed { set; get; }
        [NotMapped]
        public double BasePoints { get; set; }
        public int CustomerID { get; set; }
        public int WarehouseID { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }

    [Table("PointRedemption")]
    public class PointRedemption : PointRedemptionBase
    {
        [NotMapped]
        public int LineID { get; set; }
        [Required(ErrorMessage = "Point items are needed.")]
        public List<PointItem> PointItems { set; get; }
    }

    public class PointItemBase : ISyncEntity
    {
        [Key]
        public int ID { set; get; }
        public string LineID { set; get; }
        public int ItemID { set; get; }
        public int WarehouseID { set; get; }
        [NotMapped]
        public string ItemCode { set; get; }
        [NotMapped]
        public string ItemName { set; get; }
        public double Qty { set; get; }
        [NotMapped]
        public double BaseItemQty { set; get; }
        public int UomID { set; get; }
        [NotMapped]
        public string UomName { set; get; }
        [NotMapped]
        public double Instock { get; set; }
        [NotMapped]
        public List<SelectListItem> ItemUoms { set; get; }
        public bool Deleted { set; get; }
        [NotMapped]
        public string Process { get; set; }

        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }


    [Table("PointItem")]
    public class PointItem : PointItemBase
    {
        public int PointRedemptID { set; get; }
    }

    [Table("PointRedemptionHistory")]
    public class PointRedemptionHistory : PointRedemptionBase
    {
        public List<PointItemHistory> PointItems { set; get; }
    }
    [Table("PointItemHistory")]
    public class PointItemHistory : PointItemBase
    {
        public int PointRedemptionHistoryID { get; set; }
    }
}
