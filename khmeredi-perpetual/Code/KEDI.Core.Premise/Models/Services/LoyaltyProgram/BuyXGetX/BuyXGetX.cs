using KEDI.Core.Premise.Models.Sync;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace KEDI.Core.Premise.Models.Services.LoyaltyProgram.BuyXGetX
{
    [Table("BuyXGetX")]
    public class BuyXGetX : ISyncEntity
    {
        [Key]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int PriListID { get; set; }
        // [Column(TypeName = "Date")]
        public DateTime DateF { get; set; }
        // [Column(TypeName = "Date")]
        public DateTime DateT { get; set; }
        public bool Active { get; set; }
        public List<BuyXGetXDetail> BuyXGetXDetails { get; set; }


        //ISyncEntity
        public Guid RowId { set; get; }
        public DateTimeOffset ChangeLog { set; get; }
        public int Spk { get; set; } // Server Primary Key
        public int Cpk { get; set; } // Client Primary Key
        public bool Synced { get; set; }
        public DateTime SyncDate { get; set; }
    }
}
