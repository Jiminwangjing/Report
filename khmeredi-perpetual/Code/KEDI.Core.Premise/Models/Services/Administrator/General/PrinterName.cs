using KEDI.Core.Premise.Models.Services.Inventory;
using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Administrator.General
{
    [Table("tbPrinterName", Schema = "dbo")]
    public class PrinterName : ISyncEntity, ISelectListItem
    {
        [Key]
        public int ID { get; set; }
        [Required(ErrorMessage = "Please input name !")]
        public string Name { get; set; }
        public string MachineName { get; set; }
        public bool Delete { get; set; }
        public int OrderCount { get; set; }
        public SpiteOrder Split { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
    public enum SpiteOrder
    {
        SpiteNone = 1,
        SpiteQty = 2,
        SpiteItem = 3
    }
}
