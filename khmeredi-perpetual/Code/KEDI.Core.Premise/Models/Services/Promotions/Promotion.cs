using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace CKBS.Models.Services.Promotions
{
    [Table("tbPromotion",Schema ="dbo")]
    public class Promotion : ISyncEntity
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ID { get; set; }
         [Required(ErrorMessage ="Please input pricelist !")]
        public int PriceListID {get;set;}
        [Required(ErrorMessage ="Please input name !")]
        public string Name { get; set; }  
        public string Type { get; set; }
        public TimeSpan StartTime {get;set;}
        public TimeSpan StopTime {get;set;}
        public DateTime? StartDate { get; set; }
        public DateTime? StopDate { get; set; }

        public bool Active { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
